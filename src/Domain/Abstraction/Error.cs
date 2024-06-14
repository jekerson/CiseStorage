namespace Domain.Abstraction
{
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3
    }
    public sealed record Error
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

        private Error(string code, string description, ErrorType errorType)
        {
            Code = code;
            Description = description;
            Type = errorType;
        }

        public string Code { get; }
        public string Description { get; }
        public ErrorType Type { get; }

        public static Error Failure(string code, string description) =>
            new Error(code, description, ErrorType.Failure);

        public static Error Validation(string code, string description) =>
            new Error(code, description, ErrorType.Validation);

        public static Error NotFound(string code, string description) =>
            new Error(code, description, ErrorType.NotFound);

        public static Error Conflict(string code, string description) =>
            new Error(code, description, ErrorType.Conflict);
    }
    public class Result
    {
        protected Result(bool isSuccess, List<Error> errors)
        {
            if (isSuccess && errors.Any() || !isSuccess && !errors.Any())
            {
                throw new ArgumentException("Invalid error", nameof(errors));
            }
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public List<Error> Errors { get; }

        public static Result Success() => new(true, new List<Error> { Error.None });
        public static Result Failure(Error error) => new(false, new List<Error> { error });
        public static Result Failure(List<Error> errors) => new(false, errors);
    }

    public class Result<T> : Result
    {
        protected internal Result(bool isSuccess, List<Error> errors, T? value = default)
            : base(isSuccess, errors)
        {
            Value = value;
        }

        public T? Value { get; }
        public static Result<T> Success(T value) => new(true, new List<Error> { Error.None }, value);
        public static new Result<T> Failure(Error error) => new(false, new List<Error> { error });
        public new static Result<T> Failure(List<Error> errors) => new(false, errors);
    }


}
