using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Modules.Admins.Dtos
{
    public class AdminResponseDTO
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
