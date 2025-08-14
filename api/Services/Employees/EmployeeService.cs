// Services\Employees\EmployeeService.cs
using api.Dtos.Employee;
using api.Exceptions;               // tu NotFoundException actual
using api.Models;
using api.Repositories.Employee;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;
        public EmployeeService(IEmployeeRepository repo) => _repo = repo;

        // Helper centralizado
        private async Task<Employee> GetEmployeeOrThrowAsync(int id)
        {
            var employee = await _repo.GetByIdAsync(id, track: true);
            if (employee is null)
                throw new NotFoundException(nameof(Employee), id);
            return employee;
        }

        public async Task<List<EmployeeOutputDto>> AllEmployees()
        {
            // Proyección directa desde el repo (sin tracking)
            return await _repo.Query(track: false)
                .Select(e => new EmployeeOutputDto
                {
                    Name = e.Name,
                    LastName = e.LastName,
                    Age = e.Age,
                    Email = e.Email
                })
                .ToListAsync();
        }

        public async Task<EmployeeOutputDto?> AddEmployee(EmployeeCreateDto dto)
        {
            var employee = new Employee
            {
                Name = dto.Name,
                LastName = dto.LastName,
                Age = dto.Age,
                Address = dto.Address,
                Cel = dto.Cel,
                Email = dto.Email
            };

            await _repo.AddAsync(employee);
            await _repo.SaveChangesAsync();

            return new EmployeeOutputDto
            {
                Name = employee.Name,
                LastName = employee.LastName,
                Age = employee.Age,
                Email = employee.Email
            };
        }

        public async Task UpdateEmployee(EmployeeUpdateDto dto)
        {
            var employee = await GetEmployeeOrThrowAsync(dto.IdEmployee);

            employee.Name = dto.Name;
            employee.LastName = dto.LastName;
            employee.Age = dto.Age;
            employee.Address = dto.Address;
            employee.Cel = dto.Cel;
            employee.Email = dto.Email;

            // Si está tracked, no hace falta Update; pero no daña:
            // _repo.Update(employee);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateEmployeePartial(int id, EmployeePatchDto dto)
        {
            var employee = await GetEmployeeOrThrowAsync(id);

            if (dto.Name != null) employee.Name = dto.Name;
            if (dto.LastName != null) employee.LastName = dto.LastName;
            if (dto.Age.HasValue) employee.Age = dto.Age.Value;
            if (dto.Address != null) employee.Address = dto.Address;
            if (dto.Cel != null) employee.Cel = dto.Cel;
            if (dto.Email != null) employee.Email = dto.Email;

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteEmployee(int employeeId)
        {
            var employee = await GetEmployeeOrThrowAsync(employeeId);
            _repo.Remove(employee);
            await _repo.SaveChangesAsync();
        }
    }
}
