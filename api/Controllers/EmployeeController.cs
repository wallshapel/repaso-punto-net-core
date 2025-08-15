// Controllers\EmployeeController.cs
using api.Dtos.Employee;
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

        [HttpGet]
        public async Task<ActionResult<List<EmployeeOutputDto>>> GetAll()
            => Ok(await _service.AllEmployees());

        [HttpPost]
        public async Task<ActionResult<EmployeeOutputDto>> Create([FromBody] EmployeeCreateDto dto)
        {
            var result = await _service.AddEmployee(dto);
            return CreatedAtAction(nameof(GetAll), result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto)
        {
            await _service.UpdateEmployee(id, dto);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, [FromBody] EmployeePatchDto dto)
        {
            await _service.UpdateEmployeePartial(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteEmployee(id);
            return NoContent();
        }
    }
}
