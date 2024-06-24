using System.Reflection;

namespace Backend.Types;

public class PagingData
{
    public int Page { get; private init; } = 1;

    private static int PageSize => 10;

    public int PageOffset => (Page - 1) * PageSize;

    public static ValueTask<PagingData?> BindAsync(HttpContext context, ParameterInfo _)
    {
        const string pageKey = "page";

        if (!int.TryParse(context.Request.Query[pageKey], out var page)) page = 1;
        var result = new PagingData
        {
            Page = page
        };

        return ValueTask.FromResult<PagingData?>(result);
    }
}