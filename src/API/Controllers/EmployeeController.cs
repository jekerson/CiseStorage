﻿using API.Extensions;
using Application.Abstraction.Pagging;
using Application.DTOs.Employee;
using Application.Employees.Commands.AddEmployee;
using Application.Employees.Commands.UpdateEmployee;
using Application.Employees.Queries.GetAllEmployee;
using Application.Employees.Queries.Search.ById;
using Application.Employees.Queries.Search.ByTerm;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees(
            [FromQuery] int pageNumber = 1,
            [FromQuery] PageSizeType pageSize = PageSizeType.Medium)
        {
            var query = new GetAllEmployeesQuery(pageNumber, pageSize);
            var result = await _mediator.Send(query);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblemDetails();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var query = new GetEmployeeByIdQuery(id);
            var result = await _mediator.Send(query);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblemDetails();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployees(
            [FromQuery] string? name,
            [FromQuery] string? surname,
            [FromQuery] string? lastname, [FromQuery] string? phone,
            [FromQuery] string? email,
            [FromQuery] int pageNumber = 1,
            [FromQuery] PageSizeType pageSize = PageSizeType.Medium)
        {
            var query = new SearchEmployeesQuery(name, surname, lastname, phone, email, pageNumber, pageSize);
            var result = await _mediator.Send(query);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblemDetails();
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeDto employeeDto)
        {
            var command = new AddEmployeeCommand(employeeDto);
            var result = await _mediator.Send(command);

            return result.IsSuccess
                ? Ok()
                : result.ToProblemDetails();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeDto employeeDto)
        {
            var command = new UpdateEmployeeCommand(employeeDto);
            var result = await _mediator.Send(command);

            return result.IsSuccess
                ? Ok()
                : result.ToProblemDetails();
        }
    }
}