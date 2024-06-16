using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Abstraction
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        protected readonly ISender Sender;

        protected ApiController(ISender sender) => Sender = sender;
    }
}
