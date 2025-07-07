using SchoolManagementSystem.Modules.Classes.Entities;

namespace SchoolManagementSystem.Modules.Classes.Dtos
{
    public class ClassTeacherResponseDTO
    {
        public Guid Id { get; set; }
        public Guid IdTeacher { get; set; }
        public Guid IdClass { get; set; }
        public int Tahun { get; set; }
        public AssignmentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Related data
        public string TeacherName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public int ClassTingkat { get; set; }
    }
}
