using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Staff;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Staff
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string EmployeesCacheKey = "employeesCache";
        private const string EmployeeCacheKeyPrefix = "employeeCache_";

        public EmployeeRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<Employee>>> GetAllEmployeesAsync()
        {
            if (!_cache.TryGetValue(EmployeesCacheKey, out IEnumerable<Employee> employees))
            {
                employees = await _dbContext.Employees.AsNoTracking().Where(e => !e.IsDeleted).ToListAsync();
                _cache.Set(EmployeesCacheKey, employees, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<Employee>>.Success(employees);
        }

        public async Task<Result> AddEmployeeAsync(Employee employee)
        {
            if (await IsEmployeeExistByEmailAsync(employee.EmailAddress))
                return Result.Failure(EmployeeErrors.EmployeeAlreadyExistByEmail(employee.EmailAddress));

            if (await IsEmployeeExistByPhoneNumberAsync(employee.PhoneNumber))
                return Result.Failure(EmployeeErrors.EmployeeAlreadyExistByPhoneNumber(employee.PhoneNumber));

            await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(EmployeesCacheKey);
            return Result.Success();
        }

        public async Task<Result<Employee>> GetEmployeeByIdAsync(int id)
        {
            var cacheKey = $"{EmployeeCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out Employee employee))
            {
                employee = await _dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
                if (employee == null)
                    return Result<Employee>.Failure(EmployeeErrors.EmployeeNotFoundById(id));

                _cache.Set(cacheKey, employee, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
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

            if (existingEmployee.EmailAddress != employee.EmailAddress && await IsEmployeeExistByEmailAsync(employee.EmailAddress))
                return Result.Failure(EmployeeErrors.EmployeeAlreadyExistByEmail(employee.EmailAddress));

            if (existingEmployee.PhoneNumber != employee.PhoneNumber && await IsEmployeeExistByPhoneNumberAsync(employee.PhoneNumber))
                return Result.Failure(EmployeeErrors.EmployeeAlreadyExistByPhoneNumber(employee.PhoneNumber));

            _dbContext.Entry(existingEmployee).CurrentValues.SetValues(employee);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(EmployeesCacheKey);
            _cache.Remove($"{EmployeeCacheKeyPrefix}{employee.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteEmployeeAsync(int id)
        {
            var employee = await _dbContext.Employees.FindAsync(id);
            if (employee == null)
                return Result.Failure(EmployeeErrors.EmployeeNotFoundById(id));

            employee.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            _cache.Remove(EmployeesCacheKey);
            _cache.Remove($"{EmployeeCacheKeyPrefix}{id}");
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
