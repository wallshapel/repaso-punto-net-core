// DTOs\Employee\EmployeeUpdateDto.cs
using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Employee
{
    public class EmployeeUpdateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The Id must be greater than 0.")]
        public int IdEmployee { get; set; }

        [Required(ErrorMessage = "The name is required.")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "The last name is required.")]
        [StringLength(100, ErrorMessage = "The last name cannot exceed 100 characters.")]
        public string? LastName { get; set; }

        [Range(0, 120, ErrorMessage = "The age must be between 0 and 120.")]
        public int? Age { get; set; }

        [StringLength(100, ErrorMessage = "The address cannot exceed 100 characters.")]
        public string? Address { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "The cell phone number must have exactly 10 digits.")]
        public string? Cel { get; set; }

        [StringLength(100, ErrorMessage = "The email cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "The email is not in a valid format.")]
        public string? Email { get; set; }
    }
}