using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskManager.Application.Services;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Logging.AddConsole();

// Register application services
builder.Services.AddTransient<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ITaskService, TaskService>();
builder.Services.AddHttpLogging(o => { });

// Configure CORS with more specific settings
builder.Services.AddCors(options =>
{
    options.AddPolicy("StrictCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .WithHeaders("Content-Type", "Authorization")
              .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
              .AllowCredentials();
    });
});

// Database configuration
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Skip authentication for OPTIONS requests
            if (context.Request.Method == "OPTIONS")
            {
                context.NoResult();
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        }
    };
});

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    var xmlFiles = new[] {
        "TaskManager.Infrastructure.xml",
        "TaskManager.Domain.xml",
        "TaskManager.Application.xml",
        "TaskManager.WebApi.xml",
    };
    foreach (var xmlFile in xmlFiles)
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    }
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT token as: Bearer {your token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

var app = builder.Build();

// Special middleware to handle OPTIONS requests at the very beginning
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        context.Response.Headers.Add("Access-Control-Allow-Origin", context.Request.Headers["Origin"]);
        context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        await context.Response.CompleteAsync();
        return;
    }
    
    await next();
});

app.UseSwagger();
app.UseSwaggerUI();

// Apply CORS policy
app.UseCors("StrictCorsPolicy");

app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();