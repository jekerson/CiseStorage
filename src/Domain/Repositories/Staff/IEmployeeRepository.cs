using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Staff
{
    public interface IEmployeeRepository
    {
        Task<Result<IEnumerable<Employee>>> GetAllEmployeesAsync();
        Task<Result<IEnumerable<Employee>>> GetAllEmployeesWithPositionAsync();
        Task<Result> AddEmployeeAsync(Employee employee);
        Task<Result<Employee>> GetEmployeeByIdAsync(int id);
        Task<Result<Employee>> GetEmployeeByEmailAsync(string email);
        Task<Result<Employee>> GetEmployeeByPhoneNumberAsync(string phoneNumber);
        Task<Result> UpdateEmployeeAsync(Employee employee);
        Task<Result> DeleteEmployeeAsync(int id);
        Task<Result<IEnumerable<Employee>>> GetDeletedEmployeesAsync();
        Task<Result<Employee>> GetDeletedEmployeeByIdAsync(int id);

    }


}
