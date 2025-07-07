using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Classes.Entities;

namespace SchoolManagementSystem.Modules.Enrollments.Entities
{
    public class Enrollment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid IdStudent { get; set; }
        public Guid IdClassTeacher { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Student Student { get; set; } = null!;
        public ClassTeacher ClassTeacher { get; set; } = null!;
    }
}
