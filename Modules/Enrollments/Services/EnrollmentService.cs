using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Modules.Enrollments.Repositories;
using SchoolManagementSystem.Modules.Enrollments.Mappers;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Modules.Classes.Repositories;
using SchoolManagementSystem.Modules.Classes.Entities;
using SchoolManagementSystem.Common.Constants;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Modules.Enrollments.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassTeacherRepository _classTeacherRepository;
        private readonly DatabaseConfig _context;
        private readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            IClassTeacherRepository classTeacherRepository,
            DatabaseConfig context,
            ILogger<EnrollmentService> logger)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _classTeacherRepository = classTeacherRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<List<EnrollmentResponseDTO>> GetAllEnrollmentsAsync()
        {
            try
            {
                var enrollments = await _enrollmentRepository.GetAllAsync();
                return enrollments.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all enrollments: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<(List<EnrollmentResponseDTO> enrollments, int totalCount)> GetAllEnrollmentsPaginatedAsync(int page, int pageSize)
        {
            try
            {
                var (enrollments, totalCount) = await _enrollmentRepository.GetAllPaginatedAsync(page, pageSize);
                var enrollmentDTOs = enrollments.ToResponseDTOList();
                return (enrollmentDTOs, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated enrollments (page: {Page}, size: {Size}): {Message}", page, pageSize, ex.Message);
                throw;
            }
        }

        public async Task<EnrollmentResponseDTO?> GetEnrollmentByIdAsync(Guid id)
        {
            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(id);
                return enrollment?.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollment by ID {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<EnrollmentResponseDTO>> GetEnrollmentsByStudentIdAsync(Guid studentId)
        {
            try
            {
                var enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId);
                return enrollments.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments by student ID {StudentId}: {Message}", studentId, ex.Message);
                throw;
            }
        }

        public async Task<List<EnrollmentResponseDTO>> GetEnrollmentsByClassTeacherIdAsync(Guid classTeacherId)
        {
            try
            {
                var enrollments = await _enrollmentRepository.GetByClassTeacherIdAsync(classTeacherId);
                return enrollments.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments by class teacher ID {ClassTeacherId}: {Message}", classTeacherId, ex.Message);
                throw;
            }
        }

        public async Task<EnrollmentResponseDTO> CreateEnrollmentAsync(CreateEnrollmentDTO createEnrollmentDto)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(createEnrollmentDto.IdStudent);
                if (student == null)
                    throw new ArgumentException("Student not found");

                var existingEnrollmentAny = await _enrollmentRepository.IsStudentEnrolledInAnyClassAsync(createEnrollmentDto.IdStudent);
                if (existingEnrollmentAny)
                    throw new ArgumentException("Student is already enrolled in a class. One student can only be enrolled in one class.");

                var classTeacher = await _classTeacherRepository.GetByIdAsync(createEnrollmentDto.IdClassTeacher);
                if (classTeacher == null)
                    throw new ArgumentException("Class teacher assignment not found");

                if (classTeacher.Status != SchoolManagementSystem.Modules.Classes.Entities.AssignmentStatus.Assigned)
                    throw new ArgumentException("Teacher is not currently assigned to this class");

                var enrollment = createEnrollmentDto.ToEntity();
                var createdEnrollment = await _enrollmentRepository.CreateAsync(enrollment);

                return (await _enrollmentRepository.GetByIdAsync(createdEnrollment.Id))!.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating enrollment: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<EnrollmentResponseDTO?> UpdateEnrollmentAsync(Guid id, CreateEnrollmentDTO updateEnrollmentDto)
        {
            try
            {
                var existingEnrollment = await _enrollmentRepository.GetByIdAsync(id);
                if (existingEnrollment == null) return null;

                var student = await _studentRepository.GetByIdAsync(updateEnrollmentDto.IdStudent);
                if (student == null)
                    throw new ArgumentException("Student not found");

                var classTeacher = await _classTeacherRepository.GetByIdAsync(updateEnrollmentDto.IdClassTeacher);
                if (classTeacher == null)
                    throw new ArgumentException("Class teacher assignment not found");

                if (classTeacher.Status != SchoolManagementSystem.Modules.Classes.Entities.AssignmentStatus.Assigned)
                    throw new ArgumentException("Teacher is not currently assigned to this class");

                var duplicateEnrollment = await _enrollmentRepository.GetByStudentAndClassTeacherAsync(
                    updateEnrollmentDto.IdStudent, updateEnrollmentDto.IdClassTeacher);
                if (duplicateEnrollment != null && duplicateEnrollment.Id != id)
                    throw new ArgumentException("Student is already enrolled in this class");

                existingEnrollment.UpdateFromDTO(updateEnrollmentDto);
                var updatedEnrollment = await _enrollmentRepository.UpdateAsync(existingEnrollment);

                return updatedEnrollment.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating enrollment {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteEnrollmentAsync(Guid id)
        {
            try
            {
                return await _enrollmentRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting enrollment {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid classTeacherId)
        {
            try
            {
                return await _enrollmentRepository.IsStudentEnrolledAsync(studentId, classTeacherId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if student {StudentId} is enrolled in class {ClassTeacherId}: {Message}", studentId, classTeacherId, ex.Message);
                throw;
            }
        }

        public async Task<bool> IsTeacherAuthorizedForEnrollmentAsync(Guid teacherId, Guid enrollmentId)
        {
            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
                if (enrollment == null) return false;

                var classTeacher = await _classTeacherRepository.GetByIdAsync(enrollment.IdClassTeacher);
                return classTeacher?.IdTeacher == teacherId && classTeacher.Status == AssignmentStatus.Assigned;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if teacher {TeacherId} is authorized for enrollment {EnrollmentId}: {Message}", teacherId, enrollmentId, ex.Message);
                throw;
            }
        }

        public async Task<bool> IsTeacherAuthorizedForStudentAsync(Guid teacherId, Guid studentId)
        {
            try
            {
                var studentEnrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId);
                
                foreach (var enrollment in studentEnrollments)
                {
                    var classTeacher = await _classTeacherRepository.GetByIdAsync(enrollment.IdClassTeacher);
                    if (classTeacher?.IdTeacher == teacherId && classTeacher.Status == AssignmentStatus.Assigned)
                    {
                        return true;
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if teacher {TeacherId} is authorized for student {StudentId}: {Message}", teacherId, studentId, ex.Message);
                throw;
            }
        }

        public async Task<bool> IsTeacherAuthorizedForClassTeacherAsync(Guid teacherId, Guid classTeacherId)
        {
            try
            {
                var classTeacher = await _classTeacherRepository.GetByIdAsync(classTeacherId);
                return classTeacher?.IdTeacher == teacherId && classTeacher.Status == AssignmentStatus.Assigned;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if teacher {TeacherId} is authorized for class teacher {ClassTeacherId}: {Message}", teacherId, classTeacherId, ex.Message);
                throw;
            }
        }

        public async Task<List<EnrollmentResponseDTO>> GetEnrollmentsByTeacherIdAsync(Guid teacherId)
        {
            try
            {
                var classTeacherAssignments = await _classTeacherRepository.GetByTeacherIdAsync(teacherId);
                var enrollments = new List<Entities.Enrollment>();

                foreach (var classTeacher in classTeacherAssignments.Where(ct => ct.Status == AssignmentStatus.Assigned))
                {
                    var classEnrollments = await _enrollmentRepository.GetByClassTeacherIdAsync(classTeacher.Id);
                    enrollments.AddRange(classEnrollments);
                }

                return enrollments.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments by teacher ID {TeacherId}: {Message}", teacherId, ex.Message);
                throw;
            }
        }

        public async Task<Guid?> GetTeacherIdFromUserIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Role == UserRoles.Teacher);
                return user?.IdUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teacher ID from user ID {UserId}: {Message}", userId, ex.Message);
                throw;
            }
        }

        public async Task<Guid?> GetStudentIdFromUserIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Role == UserRoles.Student);
                return user?.IdUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student ID from user ID {UserId}: {Message}", userId, ex.Message);
                throw;
            }
        }

        public async Task<List<EnrollmentResponseDTO>> GetEnrollmentsByUserIdAsync(int userId, string role)
        {
            try
            {
                if (role == "Teacher")
                {
                    var teacherId = await GetTeacherIdFromUserIdAsync(userId);
                    if (teacherId == null)
                        throw new ArgumentException("Teacher not found for this user");
                    
                    return await GetEnrollmentsByTeacherIdAsync(teacherId.Value);
                }
                else if (role == "Student")
                {
                    var studentId = await GetStudentIdFromUserIdAsync(userId);
                    if (studentId == null)
                        throw new ArgumentException("Student not found for this user");
                    
                    return await GetEnrollmentsByStudentIdAsync(studentId.Value);
                }
                
                throw new ArgumentException("Invalid role");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments by user ID {UserId} with role {Role}: {Message}", userId, role, ex.Message);
                throw;
            }
        }
    }
}
