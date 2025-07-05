using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SchoolManagementSystem.Modules.Students.Dtos
{
    public class CreateStudentDTO
    {
        [Required]
        [StringLength(20)]
        [DefaultValue("1234567890")]
        public required string NISN { get; set; }

        [Required]
        [StringLength(100)]
        [DefaultValue("John Doe")]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [DefaultValue("john@example.com")]
        public required string Email { get; set; }

        [Required]
        [StringLength(255)]
        [DefaultValue("password123")]
        public required string Password { get; set; }
        
        [StringLength(15, MinimumLength = 10)]
        [Phone]
        [DefaultValue("081234567890")]
        public string? PhoneNumber { get; set; }
    }
}
