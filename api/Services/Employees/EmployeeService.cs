using api.Data;                       // Importa el DbContext
using api.Models;                     // Importa la clase Employee
using Microsoft.EntityFrameworkCore;  // Importa extensiones async para EF Core

namespace api.Services.Employees
{
    // Implementa la interfaz del servicio de empleados
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context; // Contexto de base de datos inyectado

        // Constructor: recibe e inyecta el contexto
        public EmployeeService(AppDbContext context) => _context = context;

        // Agrega un nuevo empleado
        public async Task<int> AddEmployee(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            return await _context.SaveChangesAsync();
        }

        // Devuelve todos los empleados
        public async Task<List<Employee>> AllEmployees() =>
            await _context.Employees.ToListAsync();

        // Elimina un empleado por ID
        public async Task<int> DeleteEmployee(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return 0;
            _context.Employees.Remove(employee);
            return await _context.SaveChangesAsync();
        }

        // Actualiza un empleado existente
        public async Task<int> UpdateEmployee(Employee employee)
        {
            _context.Employees.Update(employee);
            return await _context.SaveChangesAsync();
        }
    }
}
