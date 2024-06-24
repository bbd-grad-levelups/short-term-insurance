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
        var group = app.MapGroup("/business").AllowAnonymous();
        group.MapGet("/", GetBusiness);
        //group.MapPost("/add", AddBusiness).AddEndpointFilter<ValidationFilter<BusinessAdd>>();
    }

    private static Task<JsonHttpResult<ApiMessage<Persona>>> GetBusiness(
        ILogger<Program> logger,
        DbDataSource source,
        ICognitoService cognito) =>
        source.RunSqlQuery(logger, "Unable to get businesses", con =>
            con.QuerySingleAsync<Persona>(
                "SELECT id FROM personas WHERE id = 1",
                cognito.Get()
            ));
}