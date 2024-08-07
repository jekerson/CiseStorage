﻿using API.Extensions;
using Application.Users.Auth.Refresh;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TokenController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblemDetails();
        }
    }
}