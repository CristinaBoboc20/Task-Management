using DotNetEnv;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TaskManagement.Authentication;
using TaskManagement.Data;
using TaskManagement.Middleware;
using TaskManagement.Repositories;
using TaskManagement.Services;


var builder = WebApplication.CreateBuilder(args);


// Load environment variables from .env file
Env.Load();

var configuration = builder.Configuration;


// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

     });

// Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Management API", Version = "v1" });

    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authentication Header"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },

            new string[] {}
        }
    });
});


// Use env variables to build the connection..
var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_DATABASE")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Username={Environment.GetEnvironmentVariable("DB_USERNAME")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

// Register Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Register Task Service
builder.Services.AddScoped<ITasksService, TasksService>();
builder.Services.AddScoped<ITaskSharingService, TaskSharingService>();


// Register Task Repository
builder.Services.AddTransient<ITasksRepository, TasksRepository>();
builder.Services.AddTransient<ITaskSharingRepository, TaskSharingRepository>();

// Register Exception Middleware
builder.Services.AddScoped<ExceptionMiddleware>();

// Register Basic Authentication
builder.Services.AddAuthentication("BasicAuthentication")
       .AddScheme<AuthenticationSchemeOptions, BasicAuthentication>("BasicAuthentication", null);

builder.Services.AddAuthorization();

var app = builder.Build();

// Seed users
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedData.SeedUsers(context);
} 


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1"));
}

// Add Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
