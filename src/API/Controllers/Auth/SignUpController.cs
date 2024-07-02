using API.Extensions;
using Application.Users.Auth.Registration;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SignUpController(IMediator mediator)
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
