// Repositories\Employee\EmployeeRepository.cs
using api.Data;
using EmployeeEntity = api.Models.Employee;  // ← alias para la entidad
using Microsoft.EntityFrameworkCore;

namespace api.Repositories.Employee
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<EmployeeEntity> _set;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
            _set = context.Employees;
        }

        public async Task<EmployeeEntity?> GetByIdAsync(int id, bool track = true)
        {
            var query = track ? _set : _set.AsNoTracking();
            return await query.FirstOrDefaultAsync(e => e.IdEmployee == id);
        }

        public async Task<List<EmployeeEntity>> GetAllAsync(bool track = false)
        {
            var query = track ? _set : _set.AsNoTracking();
            return await query.ToListAsync();
        }

        public IQueryable<EmployeeEntity> Query(bool track = false)
            => track ? _set : _set.AsNoTracking();

        public async Task AddAsync(EmployeeEntity entity) => await _set.AddAsync(entity);

        public void Update(EmployeeEntity entity) => _set.Update(entity);

        public void Remove(EmployeeEntity entity) => _set.Remove(entity);

        public Task<bool> ExistsAsync(int id)
            => _set.AnyAsync(e => e.IdEmployee == id);

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
