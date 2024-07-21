using API.Extensions;
using Application.Attributes.Attribute.Query.GetAll;
using Application.Attributes.AttributeCategory.Command;
using Application.Attributes.AttributeCategory.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AttributeController(IMediator mediator) => _mediator = mediator;

        [HttpGet("category")]
        public async Task<IActionResult> GetAllAttributeCategories()
        {
            var query = new GetAllAttributeCategoriesQuery();
            var result = await _mediator.Send(query);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblemDetails();
        }

        [HttpPost("category/add")]
        public async Task<IActionResult> AddAttributeCategory([FromBody] AddAttributeCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? Ok()
                : result.ToProblemDetails();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAttributes()
        {
            var query = new GetAllAttributesQuery();
            var result = await _mediator.Send(query);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblemDetails();
        }
    }
}
