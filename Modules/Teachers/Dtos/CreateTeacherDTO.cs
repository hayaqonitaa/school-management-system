using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SchoolManagementSystem.Modules.Teachers.Dtos
{
    public class CreateTeacherDTO
    {
        [Required]
        [StringLength(20)]
        [DefaultValue("200402112023011001")]
        public required string NIP { get; set; }

        [Required]
        [StringLength(100)]
        [DefaultValue("Qonita Amani")]
        public required string FullName { get; set; }

        [Required]
        [StringLength(200)]
        [DefaultValue("Jl. Bukit Indah II No. 12")]
        public required string Alamat { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [DefaultValue("qonita.teacher@school.com")]
        public required string Email { get; set; }

        [Required]
        [StringLength(255)]
        [DefaultValue("haya123")]
        public required string Password { get; set; }
        
        [StringLength(15, MinimumLength = 10)]
        [Phone]
        [DefaultValue("081234567890")]
        public string? PhoneNumber { get; set; }
    }
}
