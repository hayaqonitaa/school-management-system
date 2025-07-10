using SchoolManagementSystem.Modules.Enrollments.Entities;

namespace SchoolManagementSystem.Modules.Enrollments.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> GetAllAsync();
        Task<(List<Enrollment> enrollments, int totalCount)> GetAllPaginatedAsync(int page, int pageSize);
        Task<Enrollment?> GetByIdAsync(Guid id);
        Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId);
        Task<List<Enrollment>> GetByClassTeacherIdAsync(Guid classTeacherId);
        Task<Enrollment?> GetByStudentAndClassTeacherAsync(Guid studentId, Guid classTeacherId);
        Task<Enrollment?> GetByStudentIdSingleAsync(Guid studentId);
        Task<Enrollment> CreateAsync(Enrollment enrollment);
        Task<Enrollment> UpdateAsync(Enrollment enrollment);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid classTeacherId);
        Task<bool> IsStudentEnrolledInAnyClassAsync(Guid studentId);
    }
}
