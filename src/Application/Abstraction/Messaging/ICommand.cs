using Domain.Abstraction;
using MediatR;

namespace Application.Abstraction.Messaging
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {

    }
}
