using Application.Abstraction.Messaging;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Repositories.Staff;

namespace Application.Employees.Queries.Search.ById
{
    public sealed class GetEmployeeByIdQueryHandler : IQueryHandler<GetEmployeeByIdQuery, Employee>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<Employee>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeResult = await _employeeRepository.GetEmployeeByIdAsync(request.Id);
            if (employeeResult.IsFailure)
            {
                return Result<Employee>.Failure(employeeResult.Error);
            }

            return Result<Employee>.Success(employeeResult.Value);
        }
    }
}
