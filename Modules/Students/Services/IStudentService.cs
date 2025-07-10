using SchoolManagementSystem.Modules.Students.Dtos;

namespace SchoolManagementSystem.Modules.Students.Services
{
    public interface IStudentService
    {
        Task<StudentResponseDTO> CreateStudentAsync(CreateStudentDTO createStudentDTO);
        Task<StudentResponseDTO?> GetStudentByIdAsync(Guid id);
        Task<List<StudentResponseDTO>> GetAllStudentsAsync();
        Task<(List<StudentResponseDTO> students, int totalCount)> GetAllStudentsPaginatedAsync(int page, int size);
        Task<StudentResponseDTO> UpdateStudentAsync(Guid id, CreateStudentDTO updateStudentDTO);
        Task<bool> DeleteStudentAsync(Guid id);
    }
}
