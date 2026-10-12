using CutCal.Recommender;
using CutCal.Services.Database;
using CutCal.Worker.Email;
using CutCal.Worker.Messaging;
using CutCal.Worker.Training;
using Microsoft.EntityFrameworkCore;

// Walk up from the current directory looking for the repo-root .env file. This works
// whether the worker is launched from the repo root or from inside backend/CutCal.Worker.
var envDir = new DirectoryInfo(Directory.GetCurrentDirectory());
for (var i = 0; i < 5 && envDir is not null; i++)
{
    var candidate = Path.Combine(envDir.FullName, ".env");
    if (File.Exists(candidate))
    {
        DotNetEnv.Env.Load(candidate);
        break;
    }
    envDir = envDir.Parent;
}

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<RabbitMqConsumerService>();

builder.Services.AddDbContext<CutCalDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton(RecommenderSettings.FromEnvironment());
builder.Services.AddSingleton<RecommenderTrainer>();
builder.Services.AddHostedService<RecommenderTrainingService>();
builder.Services.AddHostedService<RecommenderRetrainConsumerService>();

var host = builder.Build();
host.Run();
