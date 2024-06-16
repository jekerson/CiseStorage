using Domain.Abstraction;
using Microsoft.AspNetCore.Mvc;
namespace API.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToProblemDetails(this Result result)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("Cannot convert a successful result to problem details.");
            }

            var errors = result is IValidationResult validationResult
                ? validationResult.Errors
                : new[] { result.Error };

            var problemDetails = new ProblemDetails
            {
                Status = GetStatusCode(errors[0].Type),
                Title = GetTitle(errors[0].Type),
                Type = GetType(errors[0].Type),
                Detail = "One or more validation errors occurred.",
                Extensions = { ["errors"] = errors.Select(e => new { e.Code, e.Description }).ToArray() }
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }

        private static int GetStatusCode(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

        private static string GetTitle(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => "Bad Request",
                ErrorType.NotFound => "Not Found",
                ErrorType.Conflict => "Conflict",
                _ => "Internal Server Error"
            };

        private static string GetType(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => "https://datatracker.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.NotFound => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                ErrorType.Conflict => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                _ => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            };
    }
}
