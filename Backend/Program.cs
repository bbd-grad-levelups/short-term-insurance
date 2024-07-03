using Backend.Contexts;
using Backend.Jobs;
using Backend.Services;

using Hangfire;
using Hangfire.PostgreSql;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Npgsql;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var connectionStringBuilder = new NpgsqlConnectionStringBuilder()
{
  Host = builder.Configuration["DB_URL"] ?? throw new Exception("No DB_URL found"),
  Password = builder.Configuration["DB_PASSWORD"] ?? throw new Exception("No DB_PASSWORD found"),
  Username = builder.Configuration["DB_USERNAME"] ?? throw new Exception("No DB_USERNAME found"),
  Port = NpgsqlConnection.DefaultPort,
  Database = "sti",
  Pooling = true,
  MaxPoolSize = 20
};

builder.Services.AddDbContext<PersonaContext>(opt => opt.UseNpgsql(connectionStringBuilder.ConnectionString));
builder.Services.AddDbContext<LoggerContext>(opt => opt.UseNpgsql(connectionStringBuilder.ConnectionString));
builder.Services.AddControllers();

builder.Services.AddLogging();

builder.Services.AddSingleton<ISimulationService, SimulationService>();
builder.Services.AddScoped<IBankingService, BankingService>();
builder.Services.AddScoped<IStockExchangeService, StockExchangeService>();
builder.Services.AddScoped<ITaxService, TaxService>();

builder.Services.AddCors(options =>
{
  options.AddPolicy("CORS", builder =>
  {
    builder.WithOrigins(["*"])
          .WithHeaders(["Content-Type", "Authorization"])
          .WithMethods([HttpMethods.Get, HttpMethods.Post, HttpMethods.Delete, HttpMethods.Patch, HttpMethods.Put, HttpMethods.Options]).Build();
  });
});

builder.Services.AddHangfire(config =>
{
  config.UsePostgreSqlStorage(options =>
  {
    options.UseNpgsqlConnection(connectionStringBuilder.ConnectionString);
  });
});
builder.Services.AddScoped<HangfireJobs>();
builder.Services.AddHangfireServer();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Short Term Insurance API", Version = "v1" });
  c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();

var serviceProvider = app.Services;
using (var scope = serviceProvider.CreateScope())
{
  var hangfireJobs = scope.ServiceProvider.GetRequiredService<HangfireJobs>();

  RecurringJob.AddOrUpdate("TimeStep", () => hangfireJobs.TimeStep(), "*/5 * * * *");
  RecurringJob.AddOrUpdate("TestEndpoints", () => hangfireJobs.TestEndpoints(), "0 1 * * *");
}

app.UseCors("CORS");

app.MapGet("/", () => "Health Good - No need to worry Karl :)");
app.MapControllers();

app.Run();