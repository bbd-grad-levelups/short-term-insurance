using DotNext;

namespace Backend.Types;

public record ApiMessageWrapper<T>(int StatusCode, Optional<Exception> SystemError, ApiMessage<T> ApiMessage)
{
    public static ApiMessageWrapper<T> SuccessResult(T data)
    {
        return new ApiMessageWrapper<T>(StatusCodes.Status200OK, Optional.None<Exception>(),
            ApiMessage<T>.SuccessResult(data));
    }

    public static ApiMessageWrapper<T> FailureResult(string userMessage, int errorCode, Exception originalError)
    {
        return new ApiMessageWrapper<T>(errorCode, Optional.Some(originalError),
            ApiMessage<T>.FailureResult(userMessage));
    }
}