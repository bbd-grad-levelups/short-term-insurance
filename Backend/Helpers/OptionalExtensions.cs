using System.Diagnostics.CodeAnalysis;
using DotNext;

namespace Backend.Helpers;

public static class OptionalExtensions
{
    public static bool GetValue<T>(this Optional<T> result, [MaybeNullWhen(false)] out T data)
    {
        data = result.ValueOrDefault;
        return result.HasValue;
    }
}