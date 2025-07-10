using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Teachers.Entities;
using SchoolManagementSystem.Common.Models;

namespace SchoolManagementSystem.Modules.Teachers.Repositories
{
    public interface ITeacherRepository
    {
        Task<Teacher> CreateAsync(Teacher teacher);
        Task<Teacher?> GetByIdAsync(Guid id);
        Task<List<Teacher>> GetAllAsync();
        Task<(List<Teacher> teachers, int totalCount)> GetAllPaginatedAsync(int page, int pageSize);
        Task<Teacher> UpdateAsync(Teacher teacher);
        Task<bool> DeleteAsync(Guid id);
    }
}