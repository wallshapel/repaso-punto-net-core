// DTOs\Employee\EmployeeCreateDto.cs
namespace api.Dtos.Employee
{
    public class EmployeeCreateDto
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public string? Cel { get; set; }
        public string? Email { get; set; }
    }
}
