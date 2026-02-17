using Events;
using Events.User;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddWolverine(ExtensionDiscovery.ManualOnly, conf =>
{
    conf.Policies.DisableConventionalLocalRouting();
    
    conf.UseRabbitMqUsingNamedConnection("messaging")
        .AutoProvision();
    
    conf.PublishMessage<AccountRegistered>().ToRabbitTopic("KK.EVENT.CLIENT.DsF.SUCCESS.API.REGISTER", "Auth.Events");
    // conf.PublishMessage<UserRegistered>().ToRabbitTopic("KK.EVENT.ADMIN.DsF.SUCCESS.USER.DELETE", "Auth.Events");


    conf.ListenToRabbitQueue("Events.User.AccountRegistered").DefaultIncomingMessage<AccountRegistered>();
    conf.ListenToRabbitQueue("Events.User.AccountDeleted").DefaultIncomingMessage<AccountDeleted>();
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs");
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();
