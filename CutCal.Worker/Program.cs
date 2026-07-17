using CutCal.Worker.Email;
using CutCal.Worker.Messaging;

var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (File.Exists(envPath))
{
    DotNetEnv.Env.Load(envPath);
}
else if (File.Exists(".env"))
{
    DotNetEnv.Env.Load(".env");
}

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<RabbitMqConsumerService>();

var host = builder.Build();
host.Run();
