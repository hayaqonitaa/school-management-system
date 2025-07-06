namespace SchoolManagementSystem.Modules.Students.Dtos
{
    public class StudentResponseDTO
    {
        public Guid Id { get; set; }
        public string? NISN { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
