using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Entities;

namespace SchoolManagementSystem.Modules.Classes.Services
{
    public interface IClassTeacherService
    {
        Task<List<ClassTeacherResponseDTO>> GetAllClassTeachersAsync();
        Task<ClassTeacherResponseDTO?> GetClassTeacherByIdAsync(Guid id);
        Task<List<ClassTeacherResponseDTO>> GetByTeacherIdAsync(Guid teacherId);
        Task<List<ClassTeacherResponseDTO>> GetByClassIdAsync(Guid classId);
        Task<List<ClassTeacherResponseDTO>> GetByTahunAsync(int tahun);
        Task<List<ClassTeacherResponseDTO>> GetByStatusAsync(AssignmentStatus status);
        Task<ClassTeacherResponseDTO> CreateClassTeacherAsync(CreateClassTeacherDTO createClassTeacherDto);
        Task<ClassTeacherResponseDTO?> UpdateClassTeacherAsync(Guid id, CreateClassTeacherDTO updateClassTeacherDto);
        Task<bool> DeleteClassTeacherAsync(Guid id);
        Task<ClassTeacherResponseDTO?> AssignTeacherToClassAsync(Guid teacherId, Guid classId, int tahun);
        Task<bool> UnassignTeacherFromClassAsync(Guid id);
        Task<bool> AssignTeacherFromClassAsync(Guid id);
    }
}
