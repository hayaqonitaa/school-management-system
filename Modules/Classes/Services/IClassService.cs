using SchoolManagementSystem.Modules.Classes.Dtos;

namespace SchoolManagementSystem.Modules.Classes.Services
{
    public interface IClassService
    {
        Task<List<ClassResponseDTO>> GetAllClassesAsync();
        Task<ClassResponseDTO?> GetClassByIdAsync(Guid id);
        Task<List<ClassResponseDTO>> GetClassesByTingkatAsync(int tingkat);
        Task<ClassResponseDTO> CreateClassAsync(CreateClassDTO createClassDto);
        Task<ClassResponseDTO?> UpdateClassAsync(Guid id, CreateClassDTO updateClassDto);
        Task<bool> DeleteClassAsync(Guid id);
    }
}
