// Repositories\Employee\IEmployeeRepository.cs
using EmployeeEntity = api.Models.Employee;  // ← alias para la entidad para evitar colisión con el namespace del repositorio

namespace api.Repositories.Employee
{
    public interface IEmployeeRepository : IRepository<EmployeeEntity>
    {
        // Aquí podrías declarar métodos específicos, p. ej.:
        // Task<Employee?> FindByEmailAsync(string email, bool track = false);
        // Task<bool> ExistsByEmailAsync(string email);
    }
}
