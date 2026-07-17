using System.Text;
using CutCal.Common.Services;
using CutCal.Services.Auth;
using CutCal.Services.Base;
using CutCal.Services.Database;
using CutCal.Services.Messaging;
using CutCal.Services.Services;
using CutCal.WebAPI.Auth;
using CutCal.WebAPI.Filters;
using CutCal.WebAPI.Mapping;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (File.Exists(envPath))
{
    DotNetEnv.Env.Load(envPath);
}
else if (File.Exists(".env"))
{
    DotNetEnv.Env.Load(".env");
}

var builder = WebApplication.CreateBuilder(args);

// 1. HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 2. Authenticated user accessor
builder.Services.AddScoped<IAuthenticatedUserAccessor, HttpAuthenticatedUserAccessor>();

// 3. Controllers + ExceptionFilter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});

// 4. DbContext
builder.Services.AddDbContext<CutCalDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 5. Mapster
MapsterConfig.Configure();

// 6. Application services (Scoped)
builder.Services.AddScoped<ICryptoService, CryptoService>();
builder.Services.AddScoped<IAccessManager, AccessManager>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISalonService, SalonManagementService>();
builder.Services.AddScoped<ISalonCategoryService, SalonCategoryService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ISalonServiceService, SalonServiceService>();
builder.Services.AddScoped<IStaffService, StaffManagementService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

// 7. FluentValidation validators (Scoped)
builder.Services.AddValidatorsFromAssemblyContaining<CutCal.Services.Validators.RegisterRequestValidator>(ServiceLifetime.Scoped);

// 8. JWT authentication
var jwtSecret = builder.Configuration["JwtToken:SecretKey"] ?? Environment.GetEnvironmentVariable("JwtToken__SecretKey")!;
var jwtIssuer = builder.Configuration["JwtToken:Issuer"] ?? "CutCal";
var jwtAudience = builder.Configuration["JwtToken:Audience"] ?? "CutCalUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

// 9. Authorization
builder.Services.AddAuthorization();

// CORS: only the Flutter *web* build needs this (Android/Windows native HTTP clients are
// not subject to browser CORS). Explicit rule (local dev hosts only) rather than a
// blanket AllowAnyOrigin() — but any port is allowed, since `flutter run -d chrome`
// picks a random port unless --web-port is pinned, and that's inconvenient to keep in sync.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutterWebClient", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            {
                var host = new Uri(origin).Host;
                return host is "localhost" or "127.0.0.1";
            })
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// 10. OpenApi + Swagger with JWT bearer scheme
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CutCal API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 11. Automatic migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CutCalDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CutCal API v1");
});

app.UseCors("AllowFlutterWebClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
