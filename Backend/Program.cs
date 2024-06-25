using System;

using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using Backend.Helpers.Cognito;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<PersonaContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Add Swagger generation
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Short Term Insurance", Version = "v1" });

  // Define the security scheme (bearer token)
  var securityScheme = new OpenApiSecurityScheme
  {
    Name = "Authorization",
    BearerFormat = "JWT",
    Description = "JWT Authorization header using the Bearer scheme.",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Scheme = "bearer"
  };

  // Add security definition to document
  c.AddSecurityDefinition("Bearer", securityScheme);

  // Use the Bearer scheme for all operations
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

// Configure CORS
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers().AllowAnonymous();

app.Run();
