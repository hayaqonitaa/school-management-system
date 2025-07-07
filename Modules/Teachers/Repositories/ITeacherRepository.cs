using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Teachers.Entities;

namespace SchoolManagementSystem.Modules.Teachers.Repositories
{
    public interface ITeacherRepository
    {
        Task<Teacher> CreateAsync(Teacher teacher);
        Task<Teacher?> GetByIdAsync(Guid id);
        Task<List<Teacher>> GetAllAsync();
        Task<Teacher> UpdateAsync(Teacher teacher);
        Task<bool> DeleteAsync(Guid id);
    }
}