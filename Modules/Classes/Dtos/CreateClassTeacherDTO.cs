using SchoolManagementSystem.Modules.Classes.Entities;

namespace SchoolManagementSystem.Modules.Classes.Dtos
{
    public class CreateClassTeacherDTO
    {
        public Guid IdTeacher { get; set; }
        public Guid IdClass { get; set; }
        public int Tahun { get; set; }
        public AssignmentStatus Status { get; set; } = AssignmentStatus.Assigned;
    }
}
