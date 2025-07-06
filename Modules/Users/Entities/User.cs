namespace SchoolManagementSystem.Modules.Users.Entities
{
    public enum UserRole
    {
        Admin = 1,
        Teacher = 2,
        Student = 3
    }

    public class User
    {
        public int Id { get; set; }
        public Guid IdUser { get; set; }  // Reference to Student or Teacher ID (now GUID)
        public int Role { get; set; }  // 1=Admin, 2=Teacher, 3=Student

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
