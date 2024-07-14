using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Staff;
using Domain.Repositories.Staff;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Staff
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string EmployeesCacheKey = "employeesCache";
        private const string EmployeeCacheKeyPrefix = "employeeCache_";

        public EmployeeRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<Employee>>> GetAllEmployeesAsync()
        {
            var employees = await _cacheProvider.GetAsync<IEnumerable<Employee>>(EmployeesCacheKey);
            if (employees != null)
                return Result<IEnumerable<Employee>>.Success(employees);

            employees = await _dbContext.Employees.AsNoTracking().Where(e => !e.IsDeleted).ToListAsync();
            await _cacheProvider.SetAsync(EmployeesCacheKey, employees, TimeSpan.FromHours(1));
            return Result<IEnumerable<Employee>>.Success(employees);
        }

        public async Task<Result<IEnumerable<Employee>>> GetAllEmployeesWithPositionAsync()
        {
            var employees = await _dbContext.Employees
                .Include(e => e.Position)
                .AsNoTracking()
                .ToListAsync();
            return Result<IEnumerable<Employee>>.Success(employees);
        }

        public async Task<Result<int>> AddEmployeeAsync(Employee employee)
        {
            var validationResult = await ValidateEmployeeAsync(employee);
            if (!validationResult.IsSuccess)
                return Result<int>.Failure(validationResult.Error);

            await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(EmployeesCacheKey);
            return Result<int>.Success(employee.Id);
        }

        public async Task<Result<Employee>> GetEmployeeByIdAsync(int id)
        {
            var cacheKey = $"{EmployeeCacheKeyPrefix}{id}";
            var employee = await _cacheProvider.GetAsync<Employee>(cacheKey);
            if (employee != null)
                return Result<Employee>.Success(employee);

            employee = await _dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            if (employee == null)
                return Result<Employee>.Failure(EmployeeErrors.EmployeeNotFoundById(id));

            await _cacheProvider.SetAsync(cacheKey, employee, TimeSpan.FromHours(1));
            return Result<Employee>.Success(employee);
        }

        public async Task<Result<Employee>> GetEmployeeByEmailAsync(string email)
        {
            var employee = await _dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.EmailAddress == email && !e.IsDeleted);
            if (employee == null)
                return Result<Employee>.Failure(EmployeeErrors.EmployeeNotFoundByEmail(email));

            return Result<Employee>.Success(employee);
        }

        public async Task<Result<Employee>> GetEmployeeByPhoneNumberAsync(string phoneNumber)
        {
            var employee = await _dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.PhoneNumber == phoneNumber && !e.IsDeleted);
            if (employee == null)
                return Result<Employee>.Failure(EmployeeErrors.EmployeeNotFoundByPhoneNumber(phoneNumber));

            return Result<Employee>.Success(employee);
        }

        public async Task<Result> UpdateEmployeeAsync(Employee employee)
        {
            var existingEmployee = await _dbContext.Employees.FindAsync(employee.Id);
            if (existingEmployee == null)
                return Result.Failure(EmployeeErrors.EmployeeNotFoundById(employee.Id));

            var validationResult = await ValidateEmployeeAsync(employee);
            if (!validationResult.IsSuccess)
                return Result.Failure(validationResult.Error);

            _dbContext.Entry(existingEmployee).CurrentValues.SetValues(employee);
            await _dbContext.SaveChangesAsync();

            await _cacheProvider.RemoveAsync(EmployeesCacheKey);
            await _cacheProvider.RemoveAsync($"{EmployeeCacheKeyPrefix}{employee.Id}");

            return Result.Success();
        }

        public async Task<Result> DeleteEmployeeAsync(int id)
        {
            var employee = await _dbContext.Employees.FindAsync(id);
            if (employee == null)
                return Result.Failure(EmployeeErrors.EmployeeNotFoundById(id));

            employee.IsDeleted = true;
            await _dbContext.SaveChangesAsync();

            await _cacheProvider.RemoveAsync(EmployeesCacheKey);
            await _cacheProvider.RemoveAsync($"{EmployeeCacheKeyPrefix}{id}");

            return Result.Success();
        }

        public async Task<Result<IEnumerable<Employee>>> GetDeletedEmployeesAsync()
        {
            var deletedEmployees = await _dbContext.Employees.AsNoTracking().Where(e => e.IsDeleted).ToListAsync();
            return Result<IEnumerable<Employee>>.Success(deletedEmployees);
        }

        public async Task<Result<Employee>> GetDeletedEmployeeByIdAsync(int id)
        {
            var employee = await _dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted);
            if (employee == null)
                return Result<Employee>.Failure(EmployeeErrors.EmployeeNotFoundById(id));

            return Result<Employee>.Success(employee);
        }

        private async Task<Result> ValidateEmployeeAsync(Employee employee)
        {
            if (await IsEmployeeExistByEmailAsync(employee.EmailAddress))
                return Result.Failure(EmployeeErrors.EmployeeAlreadyExistByEmail(employee.EmailAddress));

            if (await IsEmployeeExistByPhoneNumberAsync(employee.PhoneNumber))
                return Result.Failure(EmployeeErrors.EmployeeAlreadyExistByPhoneNumber(employee.PhoneNumber));

            return Result.Success();
        }

        private async Task<bool> IsEmployeeExistByEmailAsync(string email)
        {
            return await _dbContext.Employees.AnyAsync(e => e.EmailAddress == email && !e.IsDeleted);
        }

        private async Task<bool> IsEmployeeExistByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbContext.Employees.AnyAsync(e => e.PhoneNumber == phoneNumber && !e.IsDeleted);
        }
    }
}