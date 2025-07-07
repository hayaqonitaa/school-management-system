namespace SchoolManagementSystem.Modules.Enrollments.Dtos
{
    public class EnrollmentResponseDTO
    {
        public Guid Id { get; set; }
        public Guid IdStudent { get; set; }
        public Guid IdClassTeacher { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Related data for display
        public string StudentName { get; set; } = string.Empty;
        public string StudentNISN { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public int ClassTingkat { get; set; }
        public int Tahun { get; set; }
    }
}
