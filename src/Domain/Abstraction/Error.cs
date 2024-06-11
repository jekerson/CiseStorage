using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);
    }

    public class Result<T> : Result
    {
        protected internal Result(bool isSuccess, Error error, T? value = default)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public T? Value { get; }
        public static Result<T> Success(T value) => new(true, Error.None, value);
        public new static Result<T> Failure(Error error) => new(false, error);
    }
}
