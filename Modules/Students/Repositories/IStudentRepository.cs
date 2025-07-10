using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Students.Entities;

namespace SchoolManagementSystem.Modules.Students.Repositories
{
    public interface IStudentRepository
    {
        Task<Student> CreateAsync(Student student);
        Task<Student?> GetByIdAsync(Guid id);
        Task<List<Student>> GetAllAsync();
        Task<(List<Student> students, int totalCount)> GetAllPaginatedAsync(int page, int size);
        Task<Student> UpdateAsync(Student student);
        Task<bool> DeleteAsync(Guid id);
        Task<int> GetTotalCountAsync();
    }
}
