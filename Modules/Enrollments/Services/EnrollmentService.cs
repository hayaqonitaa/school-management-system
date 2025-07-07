using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Modules.Enrollments.Repositories;
using SchoolManagementSystem.Modules.Enrollments.Mappers;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Modules.Classes.Repositories;

namespace SchoolManagementSystem.Modules.Enrollments.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassTeacherRepository _classTeacherRepository;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            IClassTeacherRepository classTeacherRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _classTeacherRepository = classTeacherRepository;
        }

        public async Task<List<EnrollmentResponseDTO>> GetAllEnrollmentsAsync()
        {
            var enrollments = await _enrollmentRepository.GetAllAsync();
            return enrollments.ToResponseDTOList();
        }

        public async Task<EnrollmentResponseDTO?> GetEnrollmentByIdAsync(Guid id)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            return enrollment?.ToResponseDTO();
        }

        public async Task<List<EnrollmentResponseDTO>> GetEnrollmentsByStudentIdAsync(Guid studentId)
        {
            var enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId);
            return enrollments.ToResponseDTOList();
        }

        public async Task<List<EnrollmentResponseDTO>> GetEnrollmentsByClassTeacherIdAsync(Guid classTeacherId)
        {
            var enrollments = await _enrollmentRepository.GetByClassTeacherIdAsync(classTeacherId);
            return enrollments.ToResponseDTOList();
        }

        public async Task<EnrollmentResponseDTO> CreateEnrollmentAsync(CreateEnrollmentDTO createEnrollmentDto)
        {
            // Validate student exists
            var student = await _studentRepository.GetByIdAsync(createEnrollmentDto.IdStudent);
            if (student == null)
                throw new ArgumentException("Student not found");

            // Check if student is already enrolled in any class (ONE STUDENT = ONE CLASS RULE)
            var existingEnrollmentAny = await _enrollmentRepository.IsStudentEnrolledInAnyClassAsync(createEnrollmentDto.IdStudent);
            if (existingEnrollmentAny)
                throw new ArgumentException("Student is already enrolled in a class. One student can only be enrolled in one class.");

            // Validate class teacher exists and is assigned
            var classTeacher = await _classTeacherRepository.GetByIdAsync(createEnrollmentDto.IdClassTeacher);
            if (classTeacher == null)
                throw new ArgumentException("Class teacher assignment not found");

            if (classTeacher.Status != SchoolManagementSystem.Modules.Classes.Entities.AssignmentStatus.Assigned)
                throw new ArgumentException("Teacher is not currently assigned to this class");

            // Create enrollment
            var enrollment = createEnrollmentDto.ToEntity();
            var createdEnrollment = await _enrollmentRepository.CreateAsync(enrollment);

            // Return with related data
            return (await _enrollmentRepository.GetByIdAsync(createdEnrollment.Id))!.ToResponseDTO();
        }

        public async Task<EnrollmentResponseDTO?> UpdateEnrollmentAsync(Guid id, CreateEnrollmentDTO updateEnrollmentDto)
        {
            var existingEnrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (existingEnrollment == null) return null;

            // Validate student exists
            var student = await _studentRepository.GetByIdAsync(updateEnrollmentDto.IdStudent);
            if (student == null)
                throw new ArgumentException("Student not found");

            // Validate class teacher exists and is assigned
            var classTeacher = await _classTeacherRepository.GetByIdAsync(updateEnrollmentDto.IdClassTeacher);
            if (classTeacher == null)
                throw new ArgumentException("Class teacher assignment not found");

            if (classTeacher.Status != SchoolManagementSystem.Modules.Classes.Entities.AssignmentStatus.Assigned)
                throw new ArgumentException("Teacher is not currently assigned to this class");

            // Check for duplicate enrollment (exclude current enrollment)
            var duplicateEnrollment = await _enrollmentRepository.GetByStudentAndClassTeacherAsync(
                updateEnrollmentDto.IdStudent, updateEnrollmentDto.IdClassTeacher);
            if (duplicateEnrollment != null && duplicateEnrollment.Id != id)
                throw new ArgumentException("Student is already enrolled in this class");

            // Update enrollment
            existingEnrollment.UpdateFromDTO(updateEnrollmentDto);
            var updatedEnrollment = await _enrollmentRepository.UpdateAsync(existingEnrollment);

            return updatedEnrollment.ToResponseDTO();
        }

        public async Task<bool> DeleteEnrollmentAsync(Guid id)
        {
            return await _enrollmentRepository.DeleteAsync(id);
        }

        public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid classTeacherId)
        {
            return await _enrollmentRepository.IsStudentEnrolledAsync(studentId, classTeacherId);
        }
    }
}
