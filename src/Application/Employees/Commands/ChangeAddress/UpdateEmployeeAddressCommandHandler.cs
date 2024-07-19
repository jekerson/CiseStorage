using Application.Abstraction.Messaging;
using Application.Services.Addresses;
using Domain.Abstraction;
using Domain.Repositories.Staff;

namespace Application.Employees.Commands.ChangeAddress
{
    public class UpdateEmployeeAddressCommandHandler : ICommandHandler<UpdateEmployeeAddressCommand>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAddressService _addressService;

        public UpdateEmployeeAddressCommandHandler(
            IEmployeeRepository employeeRepository,
            IAddressService addressService)
        {
            _employeeRepository = employeeRepository;
            _addressService = addressService;
        }

        public async Task<Result> Handle(UpdateEmployeeAddressCommand request, CancellationToken cancellationToken)
        {
            var employeeResult = await _employeeRepository.GetEmployeeByIdAsync(request.UpdateEmployeeAddressDto.EmployeeId);
            if (!employeeResult.IsSuccess)
                return Result.Failure(employeeResult.Error);

            var employee = employeeResult.Value;
            var updateAddressResult = await _addressService.UpdateAddressAsync(employee.AddressId, request.UpdateEmployeeAddressDto.NewAddress);
            if (!updateAddressResult.IsSuccess)
                return Result.Failure(updateAddressResult.Error);

            return Result.Success();
        }
    }
}
