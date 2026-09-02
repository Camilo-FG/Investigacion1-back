using Microsoft.AspNetCore.Http.HttpResults;

namespace Investigacion1_back.Tests.Auth;

internal static class ResultAssert
{
    public static (int StatusCode, T? Body) Read<T>(IResult result)
    {
        var status = result switch
        {
            IStatusCodeHttpResult statusCode => statusCode.StatusCode ?? StatusCodes.Status200OK,
            _ => StatusCodes.Status200OK
        };

        var body = result switch
        {
            IValueHttpResult<T> typed => typed.Value,
            IValueHttpResult untyped => untyped.Value is T value ? value : default,
            _ => default
        };

        return (status, body);
    }
}
