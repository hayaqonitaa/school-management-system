using SchoolManagementSystem.Modules.Students.Dtos;

namespace SchoolManagementSystem.Modules.Students.Services
{
    public interface IStudentService
    {
        Task<StudentResponseDTO> CreateStudentAsync(CreateStudentDTO createStudentDTO);
        Task<StudentResponseDTO?> GetStudentByIdAsync(int id);
        Task<List<StudentResponseDTO>> GetAllStudentsAsync();
        Task<StudentResponseDTO> UpdateStudentAsync(int id, CreateStudentDTO updateStudentDTO);
        Task<bool> DeleteStudentAsync(int id);
    }
}
