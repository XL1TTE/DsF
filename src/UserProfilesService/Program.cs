using Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddLogging(builder =>
{
#if DEBUG
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
#endif
});

builder.Services.AddDbContext<MongoDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("users-db") ?? throw new Exception("Connection string to MongoDb is not found!");
    options.UseMongoDB(conn, "profile-service");
});

builder.Services.AddWolverine(ExtensionDiscovery.ManualOnly, conf =>
{
    // conf.Policies.DisableConventionalLocalRouting();

    conf.CodeGeneration.AlwaysUseServiceLocationFor<MongoDbContext>();

    #region Extentions

    conf.Include<KeycloakExtentions>();

    #endregion

    conf.UseRabbitMqUsingNamedConnection("messaging")
        .AutoProvision();
});

var authority = builder.Configuration.GetValue<string>("Identity:Authority");
var clientId = builder.Configuration.GetValue<string>("Identity:ClientId");
var audience = builder.Configuration.GetValue<string>("Identity:Audience");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.Audience = clientId;
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false,
            ValidIssuer = authority,
            ValidAudience = clientId
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddOpenApi(options =>
{
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs", opt =>
    {
        opt.AddPreferredSecuritySchemes("oauth2");
        opt.AddAuthorizationCodeFlow("oauth2", flow =>
        {
            flow.ClientId = builder.Configuration.GetValue<string>("Identity:ClientId");
            flow.ClientSecret = builder.Configuration.GetValue<string>("Identity:ClientSecret");
            flow.AuthorizationUrl = builder.Configuration.GetValue<string>("Identity:AuthEndpoint");
            flow.TokenUrl = builder.Configuration.GetValue<string>("Identity:TokenEndpoint");
            flow.RedirectUri = builder.Configuration.GetValue<string>("Identity:RedirectUri");
            flow.Pkce = Pkce.Sha256;
        });
        opt.AddPasswordFlow("auth", flow =>
        {
            flow.TokenUrl = builder.Configuration.GetValue<string>("Identity:TokenEndpoint");
            flow.ClientId = builder.Configuration.GetValue<string>("Identity:ClientId");
        });
    });
}

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
