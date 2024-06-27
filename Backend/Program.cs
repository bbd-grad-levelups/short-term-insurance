using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Reflection;
using System.Text.Json;

using Backend.Helpers.Cognito;
using Backend.Helpers;

using Hangfire;
using Hangfire.PostgreSql;
using Backend.Helpers.Jobs;
using Backend.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<PersonaContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
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
  config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));// Use in-memory storage for demo purposes
});

builder.Services.AddCors(options =>
{
  options.AddPolicy("CORS", builder =>
  {
    builder.WithOrigins(["http://localhost:4200"])
          .WithHeaders(["Content-Type", "Authorization"])
          .WithMethods([HttpMethods.Get, HttpMethods.Post, HttpMethods.Delete, HttpMethods.Options]).Build();
  });
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
        {
          var json = new HttpClient().GetStringAsync(parameters.ValidIssuer + "/.well-known/jwks.json").Result;
          var keys = JsonSerializer.Deserialize<JsonWebKeySet>(json)?.Keys;
          return keys!;
        },

        ValidIssuer = builder.Configuration["COGNITO_ISSUER"] ?? throw new Exception("No COGNITO_ISSUER found"),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["COGNITO_AUDIENCE"] ?? throw new Exception("No COGNITO_AUDIENCE found"),
        ValidateAudience = false
      };
    });

builder.Services.AddLogging();
builder.Services.AddScoped<ICognitoService, CognitoService>();
builder.Services.AddSingleton<ISimulationService, SimulationService>();
builder.Configuration.Bind("BankingServiceSettings", new BankingServiceSettings());
builder.Configuration.Bind("StockExchangeSettings", new StockExchangeSettings());
builder.Services.Configure<BankingServiceSettings>(builder.Configuration.GetSection("BankingServiceSettings"));
builder.Services.Configure<StockExchangeSettings>(builder.Configuration.GetSection("StockExchangeSettings"));

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

app.UseHttpsRedirection();

app.UseCors("CORS");
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers().AllowAnonymous();

app.Run();