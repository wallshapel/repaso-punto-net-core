// DTOs\Employee\EmployeePatchDto.cs
namespace api.Dtos.Employee
{
    // Campos editables vía PATCH
    public class EmployeePatchDto
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public string? Cel { get; set; }
        public string? Email { get; set; }
    }
}
