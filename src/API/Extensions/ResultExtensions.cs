using Domain.Abstraction;

namespace API.Extensions
{
    public static class ResultExtensions
    {
        public static IResult ToProblemDetails(this Result result)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException();
            }

            return Results.Problem(
                statusCode: GetStatusCode(result.Error.Type),
                title: GetTitle(result.Error.Type),
                type: GetType(result.Error.Type),
                extensions: new Dictionary<string, object?>
                {
                    {nameof(result.Error), new[] {result.Error}}
                });
        }
        static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        static string GetTitle(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "Bad Request",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            _ => "Internal Server Error"
        };

        static string GetType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "https://datatracker.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.NotFound => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
            ErrorType.Conflict => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
            _ => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
        };
    }
}
