using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Modules.Auth.Dtos
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        
        [Required]
        public required string Password { get; set; }
    }
    
    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
    
    public class RegisterRequestDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        
        [Required]
        [StringLength(255, MinimumLength = 6)]
        public required string Password { get; set; }
        
        [Required]
        [Range(1, 3, ErrorMessage = "Role must be 1 (Admin), 2 (Teacher), or 3 (Student)")]
        public int RoleId { get; set; }
        
        // For Students
        public string? NISN { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        
        // For Teachers  
        public string? NIP { get; set; }
        public string? Address { get; set; }  // Will map to Alamat in entity
    }
}
