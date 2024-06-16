namespace Domain.Abstraction
{
    public interface IValidationResult
    {
        public static readonly Error ValidationError = new(
            "Validation.Error",
            "A validation problem occurred.",
            ErrorType.Validation);

        Error[] Errors { get; }
    }
}
