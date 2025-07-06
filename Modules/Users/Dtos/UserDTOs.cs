using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Modules.Users.Entities;
using SchoolManagementSystem.Common.Constants;

namespace SchoolManagementSystem.Modules.Users.Dtos
{
    public class CreateUserDTO
    {
        [Required]
        public int IdUser { get; set; }  // Student ID atau Teacher ID

        [Required]
        [Range(1, 3, ErrorMessage = "Role must be 1 (Admin), 2 (Teacher), or 3 (Student)")]
        public int Role { get; set; }
    }

    public class UserResponseDTO
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int Role { get; set; }
        public string RoleName => UserRoles.GetRoleName(Role); // Helper property
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
