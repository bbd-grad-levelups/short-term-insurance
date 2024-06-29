using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Reflection;
using System.Text.Json;

using Backend.Helpers;

using Hangfire;
using Hangfire.PostgreSql;
using Backend.Helpers.Jobs;
using Backend.Contexts;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionStringBuilder = new NpgsqlConnectionStringBuilder
{
  Host = builder.Configuration["DB_URL"] ?? throw new Exception("No DB_URL found"),
  Password = builder.Configuration["DB_PASSWORD"] ?? throw new Exception("No DB_PASSWORD found"),
  Username = builder.Configuration["DB_USERNAME"] ?? throw new Exception("No DB_USERNAME found"),
  Port = NpgsqlConnection.DefaultPort,
  Database = builder.Configuration["DB_DATABASE"] ?? "sti",
  Pooling = true,
  MaxPoolSize = 20,
};
string connectionString = connectionStringBuilder.ConnectionString;

builder.Services.AddControllers();
builder.Services.AddDbContext<PersonaContext>(opt => opt.UseNpgsql(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Short Term Insurance API", Version = "v1" });

  // Configure Swagger to use XML comments from assembly
  var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
  c.IncludeXmlComments(xmlPath);

  // Add security definition and requirement for JWT Bearer token
  var securityScheme = new OpenApiSecurityScheme
  {
    Name = "Authorization",
    BearerFormat = "JWT",
    Description = "JWT Authorization header using the Bearer scheme.",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Scheme = "bearer"
  };
  c.AddSecurityDefinition("Bearer", securityScheme);
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHangfire(config =>
{
  config.UsePostgreSqlStorage(connectionString);
});

builder.Services.AddCors(options =>
{
  options.AddPolicy("CORS", builder =>
  {
    builder.WithOrigins(["http://localhost:4200", "https://insurance.projects.bbdgrad.com"])
          .WithHeaders(["Content-Type", "Authorization"])
          .WithMethods([HttpMethods.Get, HttpMethods.Post, HttpMethods.Delete, HttpMethods.Options]).Build();
  });
});

builder.Services.AddLogging();
builder.Services.AddSingleton<ISimulationService, SimulationService>();
builder.Services.AddHttpClient<IBankingService, BankingService>();
builder.Services.AddHttpClient<IStockExchangeService, StockExchangeService>();
builder.Services.AddScoped<HangfireJobs>();

var app = builder.Build();

app.UseHangfireServer();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
  app.UseHangfireDashboard();
}
var serviceProvider = app.Services;
using (var scope = serviceProvider.CreateScope())
{
  var hangfireJobs = scope.ServiceProvider.GetRequiredService<HangfireJobs>();

  RecurringJob.AddOrUpdate("TimeStep", () => hangfireJobs.TimeStep(), "*/5 * * * *");
  RecurringJob.AddOrUpdate("Registration", () => hangfireJobs.RegisterCompany(100), Cron.Minutely);
}

app.UseCors("CORS");

app.MapGet("/", () => "Health is ok, real API too!");
app.MapControllers();

app.Run();