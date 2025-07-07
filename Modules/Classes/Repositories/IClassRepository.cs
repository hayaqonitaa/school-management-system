using SchoolManagementSystem.Modules.Classes.Entities;

namespace SchoolManagementSystem.Modules.Classes.Repositories
{
    public interface IClassRepository
    {
        Task<List<Class>> GetAllAsync();
        Task<Class?> GetByIdAsync(Guid id);
        Task<Class?> GetByKelasAsync(string kelas);
        Task<Class> CreateAsync(Class classEntity);
        Task<Class> UpdateAsync(Class classEntity);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<List<Class>> GetByTingkatAsync(int tingkat);
    }
}
