using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Modules.Enrollments.Repositories;
using SchoolManagementSystem.Modules.Enrollments.Mappers;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Modules.Classes.Repositories;
using SchoolManagementSystem.Modules.Classes.Entities;
using SchoolManagementSystem.Common.Constants;

namespace SchoolManagementSystem.Modules.Enrollments.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassTeacherRepository _classTeacherRepository;
        private readonly DatabaseConfig _context;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            IClassTeacherRepository classTeacherRepository,
            DatabaseConfig context)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _classTeacherRepository = classTeacherRepository;
            _context = context;
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

        public async Task<EnrollmentResponseDTO?> UpdateEnrollmentAsync(Guid id, CreateEnrollmentDTO updateEnrollmentDto)
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

        public async Task<bool> DeleteEnrollmentAsync(Guid id)
        {
            return await _enrollmentRepository.DeleteAsync(id);
        }

        public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid classTeacherId)
        {
            return await _enrollmentRepository.IsStudentEnrolledAsync(studentId, classTeacherId);
        }

        public async Task<bool> IsTeacherAuthorizedForEnrollmentAsync(Guid teacherId, Guid enrollmentId)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null) return false;

            var classTeacher = await _classTeacherRepository.GetByIdAsync(enrollment.IdClassTeacher);
            return classTeacher?.IdTeacher == teacherId && classTeacher.Status == AssignmentStatus.Assigned;
        }

        public async Task<bool> IsTeacherAuthorizedForStudentAsync(Guid teacherId, Guid studentId)
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

        public async Task<bool> IsTeacherAuthorizedForClassTeacherAsync(Guid teacherId, Guid classTeacherId)
        {
            var classTeacher = await _classTeacherRepository.GetByIdAsync(classTeacherId);
            return classTeacher?.IdTeacher == teacherId && classTeacher.Status == AssignmentStatus.Assigned;
        }

        public async Task<List<EnrollmentResponseDTO>> GetEnrollmentsByTeacherIdAsync(Guid teacherId)
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

        public async Task<Guid?> GetTeacherIdFromUserIdAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Role == UserRoles.Teacher);
            return user?.IdUser;
        }

        public async Task<Guid?> GetStudentIdFromUserIdAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Role == UserRoles.Student);
            return user?.IdUser;
        }

        public async Task<List<EnrollmentResponseDTO>> GetEnrollmentsByUserIdAsync(int userId, string role)
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
    }
}
