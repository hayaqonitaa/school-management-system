namespace SchoolManagementSystem.Modules.Teachers.Dtos
{
    public class TeacherResponseDTO
    {
        public Guid Id { get; set; }
        public string? NIP { get; set; }
        public string? FullName { get; set; }
        public string? Alamat { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
