// Services\Employees\IEmployeeService.cs
using api.Dtos.Employee;

namespace api.Services.Employees
{
    public interface IEmployeeService
    {
        Task<List<EmployeeOutputDto>> AllEmployees();
        Task<EmployeeOutputDto> AddEmployee(EmployeeCreateDto dto);
        Task UpdateEmployee(int id, EmployeeUpdateDto dto);
        Task UpdateEmployeePartial(int id, EmployeePatchDto dto);
        Task DeleteEmployee(int employeeId);
    }
}
