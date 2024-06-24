using System.Data.Common;
using Backend.Types;
using DotNext;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Backend.Helpers;

public static class ResponseHelper
{
    /// <summary>
    /// Maps the result to the apropriate error message and error code.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The result of the query.</param>
    /// <param name="errorMessage">The error message to be included in the ApiMessageWrapper if the result is a failure.</param>
    /// <param name="errorCode">The error code to be included in the ApiMessageWrapper if the result is a failure.</param>
    /// <returns>An ApiMessageWrapper object representing the result of the query.</returns>
    private static ApiMessageWrapper<T> QueryResultMapper<T>(Result<T> result, string errorMessage, int errorCode)
    {
        return result.Convert(ApiMessageWrapper<T>.SuccessResult)
            .OrInvoke(e => ApiMessageWrapper<T>.FailureResult(errorMessage, errorCode, e));
    }

    /// <summary>
    /// Wrapper sql query in error handling and in a connection .
    /// </summary>
    /// <typeparam name="T">The type of the result expected from the SQL query.</typeparam>
    /// <param name="source">The database source to run the query on.</param>
    /// <param name="logger">The logger to use for logging any errors.</param>
    /// <param name="message">The message to log in case of an error.</param>
    /// <param name="func">The function to execute the SQL query and return the result.</param>
    /// <returns>A JsonHttpResult containing an ApiMessage with the result of the SQL query, or an error message if an error occurred.</returns>
    public static async Task<JsonHttpResult<ApiMessage<T>>> RunSqlQuery<T>(this DbDataSource source, ILogger logger,
        string message, Func<DbConnection, Task<T>> func)
    {
        var openConnection = source.CreateConnection();
        var result = await func.TryInvokeAsync(openConnection);
        var apiMessage = QueryResultMapper(result, message, StatusCodes.Status500InternalServerError);
        if (apiMessage.SystemError.GetValue(out var error)) logger.LogError(error, "Error occured from run sql: ");
        var jsonHttpResult = TypedResults.Json(apiMessage.ApiMessage, statusCode: apiMessage.StatusCode);

        // Flip this kill the connections, but they idle for a bit so when we have many connections that are just idle we know why.
        await openConnection.CloseAsync();
        await openConnection.DisposeAsync();
        return jsonHttpResult;
    }
}