using SchoolManagementSystem.Modules.Students.Dtos;

namespace SchoolManagementSystem.Modules.Students.Services
{
    public interface IStudentService
    {
        Task<StudentResponseDTO> CreateStudentAsync(CreateStudentDTO createStudentDTO);
        Task<StudentResponseDTO?> GetStudentByIdAsync(Guid id);
        Task<StudentResponseDTO?> GetStudentByNISNAsync(string nisn);
        Task<StudentResponseDTO?> GetStudentByEmailAsync(string email);
        Task<List<StudentResponseDTO>> GetAllStudentsAsync();
        Task<StudentResponseDTO> UpdateStudentAsync(Guid id, CreateStudentDTO updateStudentDTO);
        Task<bool> DeleteStudentAsync(Guid id);
    }
}
