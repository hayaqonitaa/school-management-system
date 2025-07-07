using SchoolManagementSystem.Modules.Enrollments.Dtos;

namespace SchoolManagementSystem.Modules.Enrollments.Services
{
    public interface IEnrollmentService
    {
        Task<List<EnrollmentResponseDTO>> GetAllEnrollmentsAsync();
        Task<EnrollmentResponseDTO?> GetEnrollmentByIdAsync(Guid id);
        Task<List<EnrollmentResponseDTO>> GetEnrollmentsByStudentIdAsync(Guid studentId);
        Task<List<EnrollmentResponseDTO>> GetEnrollmentsByClassTeacherIdAsync(Guid classTeacherId);
        Task<EnrollmentResponseDTO> CreateEnrollmentAsync(CreateEnrollmentDTO createEnrollmentDto);
        Task<EnrollmentResponseDTO?> UpdateEnrollmentAsync(Guid id, CreateEnrollmentDTO updateEnrollmentDto);
        Task<bool> DeleteEnrollmentAsync(Guid id);
        Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid classTeacherId);
    }
}
