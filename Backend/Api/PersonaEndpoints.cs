using System.Data.Common;

using Backend.Helpers;
using Backend.Helpers.Cognito;
using Backend.Types;
using Backend.Types.Endpoint;

using Dapper;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Backend.Api;

public static class PersonaEndpoints
{
  public static void RegisterEndpoints(IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/persona").AllowAnonymous();
    group.MapGet("/", GetPersona);
  }

  private static Task<JsonHttpResult<ApiMessage<Persona>>> GetPersona(
      ILogger<Program> logger,
      DbDataSource source,
      ICognitoService cognito) =>
      source.RunSqlQuery(logger, "Unable to get personas", con =>
          con.QuerySingleAsync<Persona>(
              "SELECT id FROM personas WHERE id = 1",
              cognito.Get()
          ));

}