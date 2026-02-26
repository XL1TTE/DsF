using Common.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Persistence.Contexts;
using Persistence.Documents;
using Persistence.Repositories;
using Scalar.AspNetCore;
using Wolverine;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
                .AddXmlSerializerFormatters();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ContentDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("content-db") ?? throw new Exception("Connection string to MongoDb is not found!");
    options.UseMongoDB(conn, "content-service");
});

builder.Services.AddScoped<IRepository<RaceDocument>, RacesRepository>();
builder.Services.AddUnitOfWorkFor<ContentDbContext>();

builder.Services.AddWolverine(ExtensionDiscovery.ManualOnly, conf =>
{
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs");
}

var uploadsFolder = Path.Combine(builder.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadsFolder);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsFolder),
    RequestPath = "/uploads"
});

app.MapControllers();

app.Run();

