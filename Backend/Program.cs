using System.Data.Common;
using System.Text.Json;

using Backend.Api;
using Backend.Helpers.Cognito;
using Backend.Types;
using Backend.Types.Endpoint;

using Dapper;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("OurCors", builder =>
  {
    builder.WithOrigins(["http://localhost:4200"])
          .WithHeaders(["Content-Type", "Authorization"])
          .WithMethods([HttpMethods.Get, HttpMethods.Post, HttpMethods.Delete, HttpMethods.Options]).Build();
  });
});

var connectionString = new NpgsqlConnectionStringBuilder
{
  Host = builder.Configuration["DB_URL"] ?? throw new Exception("No DB_URL found"),
  Password = builder.Configuration["DB_PASSWORD"] ?? throw new Exception("No DB_PASSWORD found"),
  Username = builder.Configuration["DB_USERNAME"] ?? throw new Exception("No DB_USERNAME found"),
  Port = builder.Configuration.GetValue<int?>("DB_PORT") ?? NpgsqlConnection.DefaultPort,
  Database = builder.Configuration["DB_DATABASE"] ?? "bem",
  Pooling = true,
  MaxPoolSize = 20,
};
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

var dataSource = NpgsqlDataSource.Create(connectionString);
DefaultTypeMap.MatchNamesWithUnderscores = true;
builder.Services.AddSingleton<DbDataSource>(_ => dataSource);

builder.Services.AddScoped<ICognitoService, CognitoService>();

builder.Services.AddLogging();
var app = builder.Build();

// Use CORS
app.UseCors("OurCors");
app.UseAuthorization();
app.UseAuthentication();
app.UseMiddleware<CognitoMiddleware>();

app.MapGet("/", () => "Health is ok!").AllowAnonymous();
var group = app.MapGroup("/");

PersonaEndpoints.RegisterEndpoints(group);
app.Run();