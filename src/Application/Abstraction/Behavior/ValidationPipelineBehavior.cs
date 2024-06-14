using Domain.Abstraction;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Abstraction.Behavior
{
    internal sealed class ValidationPipelineBehavior<TRequest, TResponse>(
        IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TResponse : Result, new()
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(request, cancellationToken);

            if (validationFailures.Length != 0)
            {
                var errors = validationFailures
                    .Select(failure => Error.Validation(failure.PropertyName, failure.ErrorMessage))
                    .ToList();

                return Result.Failure(errors) as TResponse;
            }

            return await next();
        }

        private async Task<ValidationFailure[]> ValidateAsync(TRequest request, CancellationToken cancellationToken)
        {
            if (!validators.Any())
            {
                return Array.Empty<ValidationFailure>();
            }

            var context = new ValidationContext<TRequest>(request);

            ValidationResult[] validationResults = await Task.WhenAll(
                validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            ValidationFailure[] validationFailures = validationResults
                .Where(validationResult => !validationResult.IsValid)
                .SelectMany(validationResult => validationResult.Errors)
                .ToArray();

            return validationFailures;
        }
    }
}
