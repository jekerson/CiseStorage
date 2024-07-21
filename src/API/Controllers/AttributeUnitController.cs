using API.Extensions;
using Application.Attributes.AttributeUnit.Command.AddUnit;
using Application.Attributes.AttributeValueType;
using Application.Attributes.UnitCategories.Command;
using Application.Attributes.UnitCategories.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeUnitController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AttributeUnitController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddAttributeUnit([FromBody] AddAttributeUnitCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
             ? Ok()
             : result.ToProblemDetails();
        }

        [HttpGet("category")]
        public async Task<IActionResult> GetAllUnitCategories()
        {
            var query = new GetAllUnitCategoriesQuery();
            var result = await _mediator.Send(query);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblemDetails();
        }

        [HttpPost("category/add")]
        public async Task<IActionResult> AddUnitCategory([FromBody] AddUnitCategoryCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess
                ? Ok()
                : result.ToProblemDetails();
        }

        [HttpGet("attribute-datatype")]
        public async Task<IActionResult> GetAllAttributeValueTypes()
        {
            var query = new GetAllAttributeValueTypesQuery();
            var result = await _mediator.Send(query);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblemDetails();
        }
    }
}
