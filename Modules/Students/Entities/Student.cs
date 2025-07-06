namespace SchoolManagementSystem.Modules.Students.Entities
{
    public class Student
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string NISN { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
