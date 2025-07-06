using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SchoolManagementSystem.Modules.Teachers.Dtos
{
    public class CreateTeacherDTO
    {
        [Required]
        [StringLength(20)]
        [DefaultValue("198501012023011001")]
        public required string NIP { get; set; }

        [Required]
        [StringLength(100)]
        [DefaultValue("Jane Smith")]
        public required string FullName { get; set; }

        [Required]
        [StringLength(200)]
        [DefaultValue("Jl. Pendidikan No. 123, Jakarta")]
        public required string Alamat { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [DefaultValue("jane.teacher@school.com")]
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
