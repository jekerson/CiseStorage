using Application.Abstraction.Messaging;
using Domain.Abstraction;
using Domain.Repositories.Staff;

namespace Application.Employees.Commands.ChangePosition
{
    public class UpdateEmployeePositionCommandHandler : ICommandHandler<UpdateEmployeePositionCommand>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public UpdateEmployeePositionCommandHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result> Handle(UpdateEmployeePositionCommand request, CancellationToken cancellationToken)
        {
            var employeeResult = await _employeeRepository.GetEmployeeByIdAsync(request.UpdateEmployeePositionDto.EmployeeId);
            if (!employeeResult.IsSuccess)
                return Result.Failure(employeeResult.Error);

            var employee = employeeResult.Value;
            employee.PositionId = request.UpdateEmployeePositionDto.NewPositionId;

            return await _employeeRepository.UpdateEmployeeAsync(employee);
        }
    }
}
