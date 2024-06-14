using Domain.Abstraction;
using MediatR;

namespace Application.Abstraction.Messaging
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
