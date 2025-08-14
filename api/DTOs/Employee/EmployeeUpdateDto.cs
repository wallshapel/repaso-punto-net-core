// DTOs\Employee\EmployeeUpdateDto.cs
namespace api.Dtos.Employee
{
    public class EmployeeUpdateDto
    {
        public int IdEmployee { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public string? Cel { get; set; }
        public string? Email { get; set; }
    }
}
