using System.Text.Json.Serialization;
using DotNext;
using DotNext.Text.Json;

namespace Backend.Types;

public record ApiMessage<T>(
    bool Success,
    [property: JsonConverter(typeof(OptionalConverterFactory))]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Optional<string> Message,
    [property: JsonConverter(typeof(OptionalConverterFactory))]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Optional<T> Data)
{
    public static ApiMessage<T> SuccessResult(T data)
    {
        return new ApiMessage<T>(true, Optional.None<string>(), data);
    }

    public static ApiMessage<T> FailureResult(string message)
    {
        return new ApiMessage<T>(false, message, Optional.None<T>());
    }

    public static ApiMessage<T> FailureResultWithData(string message, T data)
    {
        return new ApiMessage<T>(false, message, data);
    }
}