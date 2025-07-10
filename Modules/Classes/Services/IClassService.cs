using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Common.Models;

namespace SchoolManagementSystem.Modules.Classes.Services
{
    public interface IClassService
    {
        Task<List<ClassResponseDTO>> GetAllClassesAsync();
        Task<(List<ClassResponseDTO> classes, int totalCount)> GetAllClassesPaginatedAsync(int page, int pageSize);
        Task<ClassResponseDTO?> GetClassByIdAsync(Guid id);
        Task<List<ClassResponseDTO>> GetClassesByTingkatAsync(int tingkat);
        Task<ClassResponseDTO> CreateClassAsync(CreateClassDTO createClassDto);
        Task<ClassResponseDTO?> UpdateClassAsync(Guid id, CreateClassDTO updateClassDto);
        Task<bool> DeleteClassAsync(Guid id);
    }
}
