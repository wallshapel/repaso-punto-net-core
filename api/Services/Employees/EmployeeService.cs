// Services\Employees\EmployeeService.cs
using api.Dtos.Employee;
using api.Exceptions;
using api.Models;
using api.Repositories.Employee;
using api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;
        private readonly IObjectMapper _mapper;

        public EmployeeService(IEmployeeRepository repo, IObjectMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

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
            return await _repo.ProjectAsync(e => new EmployeeOutputDto
            {
                Name = e.Name,
                LastName = e.LastName,
                Age = e.Age,
                Email = e.Email
            }, track: false);
        }

        public async Task<EmployeeOutputDto> AddEmployee(EmployeeCreateDto dto)
        {
            // ← USO DEL MAPPER: DTO → Entity (crea instancia)
            var employee = _mapper.Map<EmployeeCreateDto, Employee>(dto);
            await _repo.AddAsync(employee);
            await _repo.SaveChangesAsync();
            // ← USO DEL MAPPER: Entity → DTO de salida
            return _mapper.Map<Employee, EmployeeOutputDto>(employee);
        }

        public async Task UpdateEmployee(int id, EmployeeUpdateDto dto)
        {
            if (id != dto.IdEmployee)
                throw new ArgumentException("Route id and body id do not match.");

            var employee = await GetEmployeeOrThrowAsync(dto.IdEmployee);
            // ← USO DEL MAPPER: sobrescribe propiedades (ignora IdEmployee)
            _mapper.MapInto(dto, employee, ignoreNulls: false, "IdEmployee");
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateEmployeePartial(int id, EmployeePatchDto dto)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            // ← USO DEL MAPPER: aplica solo propiedades NO nulas (ideal para PATCH)
            _mapper.MapInto(dto, employee, ignoreNulls: true, "IdEmployee");
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
