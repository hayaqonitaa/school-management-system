namespace SchoolManagementSystem.Modules.Classes.Dtos
{
    public class CreateClassDTO
    {
        public string Kelas { get; set; } = string.Empty; // 1A, 1B, 2A, etc.
        public int Tingkat { get; set; } // 1, 2, 3, etc.
    }
}
