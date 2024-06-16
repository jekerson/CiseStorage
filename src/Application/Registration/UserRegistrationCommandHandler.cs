using Application.Abstraction.Messaging;
using Application.Services.Password;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Repositories.Role_permission;
using Domain.Repositories.Staff;

namespace Application.Registration
{
    public sealed class UserRegistrationCommandHandler : ICommandHandler<UserRegistrationCommand, UserInfo>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IPasswordService _passwordService;

        public UserRegistrationCommandHandler(
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _passwordService = passwordService;
        }

        public async Task<Result<UserInfo>> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
        {
            var employeeResult = await _employeeRepository.GetEmployeeByIdAsync(request.EmployeeId);
            if (!employeeResult.IsSuccess)
                return Result<UserInfo>.Failure(employeeResult.Error);

            var roleResult = await _roleRepository.GetRoleByIdAsync(request.RoleId);
            if (!roleResult.IsSuccess)
                return Result<UserInfo>.Failure(roleResult.Error);

            var salt = _passwordService.GenerateSalt();
            var hashedPassword = _passwordService.HashPassword(request.Password, salt);

            var userInfo = new UserInfo
            {
                EmployeeId = request.EmployeeId,
                Username = request.Username,
                Salt = salt,
                HashedPassword = hashedPassword,
                IsDeleted = false
            };

            var addUserResult = await _userRepository.AddUserAsync(userInfo);
            if (!addUserResult.IsSuccess)
                return Result<UserInfo>.Failure(addUserResult.Error);

            var userRole = new UserRole
            {
                UserInfoId = userInfo.Id,
                RoleId = request.RoleId
            };
            var userRoleResult = await _userRoleRepository.AddUserRoleAsync(userRole);
            if (!userRoleResult.IsSuccess)
                return Result<UserInfo>.Failure(userRoleResult.Error);

            return Result<UserInfo>.Success(userInfo);
        }
    }
}
