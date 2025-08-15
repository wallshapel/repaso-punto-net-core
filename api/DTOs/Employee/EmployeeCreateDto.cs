// DTOs\Employee\EmployeeCreateDto.cs
using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Employee
{
    public class EmployeeCreateDto
    {
        [Required(ErrorMessage = "Name is required..")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        public string? LastName { get; set; }

        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120.")]
        public int? Age { get; set; }

        [StringLength(100, ErrorMessage = "The address cannot exceed 100 characters.")]
        public string? Address { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "The cell phone number must have exactly 10 digits.")]
        public string? Cel { get; set; }

        [StringLength(100, ErrorMessage = "The email address cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "The email address is not in a valid format.")]
        public string? Email { get; set; }
    }
}

