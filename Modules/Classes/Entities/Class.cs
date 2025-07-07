namespace SchoolManagementSystem.Modules.Classes.Entities
{
    public class Class
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Kelas { get; set; } = string.Empty; // 1A, 1B, 2A, etc.
        public int Tingkat { get; set; } // 1, 2, 3, etc.
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
