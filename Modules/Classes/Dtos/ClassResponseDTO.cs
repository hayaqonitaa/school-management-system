namespace SchoolManagementSystem.Modules.Classes.Dtos
{
    public class ClassResponseDTO
    {
        public Guid Id { get; set; }
        public string Kelas { get; set; } = string.Empty;
        public int Tingkat { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
