using api.Models;

namespace api.Services.Employees
{
    public interface IEmployeeService
    {
        Task<List<Employee>> AllEmployees();
        Task<int> AddEmployee(Employee employee);
        Task<int> UpdateEmployee(Employee employee);
        Task<int> DeleteEmployee(int employeeId);
    }
}
