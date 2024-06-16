using API.Extensions;
using Application.Registration;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SignInController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] UserRegistrationCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess

                ? Ok()
                : result.ToProblemDetails();
        }
    }
}
