using SchoolManagementSystem.Modules.Teachers.Entities;

namespace SchoolManagementSystem.Modules.Classes.Entities
{
    public enum AssignmentStatus
    {
        Assigned = 1,
        Unassigned = 2
    }

    public class ClassTeacher
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid IdTeacher { get; set; }
        public Guid IdClass { get; set; }
        public int Tahun { get; set; }
        public AssignmentStatus Status { get; set; } = AssignmentStatus.Assigned;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Teacher Teacher { get; set; } = null!;
        public Class Class { get; set; } = null!;
    }
}
