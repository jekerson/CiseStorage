using Application.Abstraction;
using Application.Abstraction.Messaging;
using Application.Services.Addresses;
using Application.Services.Audit.Staff;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Enum;
using Domain.Repositories.Staff;

namespace Application.Employees.Commands.AddEmployee
{
    public class AddEmployeeCommandHandler : ICommandHandler<AddEmployeeCommand>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAddressService _addressService;
        private readonly IPositionRepository _positionRepository;
        private readonly IEmployeeAuditService _employeeAuditService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;


        public AddEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IAddressService addressService,
            IPositionRepository positionRepository,
            IEmployeeAuditService employeeAuditService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _addressService = addressService;
            _positionRepository = positionRepository;
            _employeeAuditService = employeeAuditService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeDto = request.EmployeeDto;

            _unitOfWork.BeginTransaction();

            try
            {
                var userResult = await _userRepository.GetUserByIdAsync(employeeDto.UserId);
                if (!userResult.IsSuccess)
                    return Result.Failure(userResult.Error);

                var positionResult = await _positionRepository.GetPositionByIdAsync(employeeDto.PositionId);
                if (!positionResult.IsSuccess)
                    return Result.Failure(positionResult.Error);

                var position = positionResult.Value;

                var addressResult = await _addressService.CreateAddressAsync(employeeDto.AddAddressDto);
                if (!addressResult.IsSuccess)
                    return Result.Failure(addressResult.Error);

                var address = addressResult.Value;

                var employee = new Employee
                {
                    Name = employeeDto.Name,
                    Surname = employeeDto.Surname,
                    Lastname = employeeDto.Lastname,
                    PhoneNumber = employeeDto.Phone,
                    EmailAddress = employeeDto.Email,
                    Sex = employeeDto.Sex,
                    Age = employeeDto.Age,
                    PositionId = position.Id,
                    AddressId = address.Id
                };

                var employeeResult = await _employeeRepository.AddEmployeeAsync(employee);
                if (!employeeResult.IsSuccess)
                    return Result.Failure(employeeResult.Error);

                var auditResult = await _employeeAuditService.AddEmployeeAuditAsync(ActionType.Insert, employeeDto.UserId, employee.Id);
                if (!auditResult.IsSuccess)
                    return Result.Failure(auditResult.Error);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}