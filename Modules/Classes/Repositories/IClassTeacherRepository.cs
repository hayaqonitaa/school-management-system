using SchoolManagementSystem.Modules.Classes.Entities;

namespace SchoolManagementSystem.Modules.Classes.Repositories
{
    public interface IClassTeacherRepository
    {
        Task<List<ClassTeacher>> GetAllAsync();
        Task<ClassTeacher?> GetByIdAsync(Guid id);
        Task<List<ClassTeacher>> GetByTeacherIdAsync(Guid teacherId);
        Task<List<ClassTeacher>> GetByClassIdAsync(Guid classId);
        Task<List<ClassTeacher>> GetByTahunAsync(int tahun);
        Task<List<ClassTeacher>> GetByStatusAsync(AssignmentStatus status);
        Task<ClassTeacher?> GetByTeacherAndYearAsync(Guid teacherId, int tahun);
        Task<ClassTeacher> CreateAsync(ClassTeacher classTeacher);
        Task<ClassTeacher> UpdateAsync(ClassTeacher classTeacher);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
