using api.Models;
using api.Services.Employees;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service) => _service = service;

        // GET: api/employee
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAll()
            => Ok(await _service.AllEmployees());

        // POST: api/employee
        [HttpPost]
        public async Task<ActionResult<Employee>> Create([FromBody] Employee employee)
        {
            var affected = await _service.AddEmployee(employee);
            if (affected <= 0) return BadRequest();
            // EF Core rellena la PK (IdEmployee) después de SaveChanges
            return Created($"api/employee/{employee.IdEmployee}", employee);
        }

        // PUT: api/employee/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Employee employee)
        {
            if (id != employee.IdEmployee) return BadRequest("Id mismatch");
            var affected = await _service.UpdateEmployee(employee);
            if (affected <= 0) return NotFound();
            return NoContent();
        }

        // DELETE: api/employee/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var affected = await _service.DeleteEmployee(id);
            if (affected <= 0) return NotFound();
            return NoContent();
        }
    }
}
