namespace SchoolManagementSystem.Modules.Teachers.Entities
{
    public class Teacher
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string NIP { get; set; }
        public required string FullName { get; set; }
        public required string Alamat { get; set; }  // Keep consistent with DTO
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
