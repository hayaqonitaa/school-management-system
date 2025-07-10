using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Entities;
using SchoolManagementSystem.Modules.Classes.Repositories;
using SchoolManagementSystem.Modules.Classes.Mappers;
using SchoolManagementSystem.Modules.Teachers.Repositories;

namespace SchoolManagementSystem.Modules.Classes.Services
{
    public class ClassTeacherService : IClassTeacherService
    {
        private readonly IClassTeacherRepository _classTeacherRepository;
        private readonly IClassRepository _classRepository;
        private readonly ITeacherRepository _teacherRepository;

        public ClassTeacherService(
            IClassTeacherRepository classTeacherRepository,
            IClassRepository classRepository,
            ITeacherRepository teacherRepository)
        {
            _classTeacherRepository = classTeacherRepository;
            _classRepository = classRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task<List<ClassTeacherResponseDTO>> GetAllClassTeachersAsync()
        {
            var classTeachers = await _classTeacherRepository.GetAllAsync();
            return classTeachers.ToResponseDTOList();
        }

        public async Task<(List<ClassTeacherResponseDTO> classTeachers, int totalCount)> GetAllClassTeachersPaginatedAsync(int page, int pageSize)
        {
            var (classTeachers, totalCount) = await _classTeacherRepository.GetAllPaginatedAsync(page, pageSize);
            var classTeacherDTOs = classTeachers.ToResponseDTOList();
            return (classTeacherDTOs, totalCount);
        }

        public async Task<ClassTeacherResponseDTO?> GetClassTeacherByIdAsync(Guid id)
        {
            var classTeacher = await _classTeacherRepository.GetByIdAsync(id);
            return classTeacher?.ToResponseDTO();
        }

        public async Task<List<ClassTeacherResponseDTO>> GetByTeacherIdAsync(Guid teacherId)
        {
            var classTeachers = await _classTeacherRepository.GetByTeacherIdAsync(teacherId);
            return classTeachers.ToResponseDTOList();
        }

        public async Task<List<ClassTeacherResponseDTO>> GetByClassIdAsync(Guid classId)
        {
            var classTeachers = await _classTeacherRepository.GetByClassIdAsync(classId);
            return classTeachers.ToResponseDTOList();
        }

        public async Task<List<ClassTeacherResponseDTO>> GetByTahunAsync(int tahun)
        {
            var classTeachers = await _classTeacherRepository.GetByTahunAsync(tahun);
            return classTeachers.ToResponseDTOList();
        }

        public async Task<List<ClassTeacherResponseDTO>> GetByStatusAsync(AssignmentStatus status)
        {
            var classTeachers = await _classTeacherRepository.GetByStatusAsync(status);
            return classTeachers.ToResponseDTOList();
        }

        public async Task<ClassTeacherResponseDTO> CreateClassTeacherAsync(CreateClassTeacherDTO createClassTeacherDto)
        {
            // Validate teacher exists
            var teacher = await _teacherRepository.GetByIdAsync(createClassTeacherDto.IdTeacher);
            if (teacher == null)
                throw new ArgumentException("Teacher not found");

            // Validate class exists
            var classEntity = await _classRepository.GetByIdAsync(createClassTeacherDto.IdClass);
            if (classEntity == null)
                throw new ArgumentException("Class not found");

            // Check if teacher already assigned to any class in the same year
            var existingAssignment = await _classTeacherRepository.GetByTeacherAndYearAsync(
                createClassTeacherDto.IdTeacher, createClassTeacherDto.Tahun);
            if (existingAssignment != null && existingAssignment.Status == AssignmentStatus.Assigned)
                throw new ArgumentException("Teacher is already assigned to a class in this year");

            // Create class teacher assignment
            var classTeacher = createClassTeacherDto.ToEntity();
            var createdClassTeacher = await _classTeacherRepository.CreateAsync(classTeacher);

            return createdClassTeacher.ToResponseDTO();
        }

        public async Task<ClassTeacherResponseDTO?> UpdateClassTeacherAsync(Guid id, CreateClassTeacherDTO updateClassTeacherDto)
        {
            var existingClassTeacher = await _classTeacherRepository.GetByIdAsync(id);
            if (existingClassTeacher == null) return null;

            // Validate teacher exists
            var teacher = await _teacherRepository.GetByIdAsync(updateClassTeacherDto.IdTeacher);
            if (teacher == null)
                throw new ArgumentException("Teacher not found");

            // Validate class exists
            var classEntity = await _classRepository.GetByIdAsync(updateClassTeacherDto.IdClass);
            if (classEntity == null)
                throw new ArgumentException("Class not found");

            // Check if teacher already assigned to any other class in the same year (exclude current assignment)
            var existingAssignment = await _classTeacherRepository.GetByTeacherAndYearAsync(
                updateClassTeacherDto.IdTeacher, updateClassTeacherDto.Tahun);
            if (existingAssignment != null && existingAssignment.Id != id && existingAssignment.Status == AssignmentStatus.Assigned)
                throw new ArgumentException("Teacher is already assigned to another class in this year");

            // Update class teacher
            existingClassTeacher.UpdateFromDTO(updateClassTeacherDto);
            var updatedClassTeacher = await _classTeacherRepository.UpdateAsync(existingClassTeacher);

            return updatedClassTeacher.ToResponseDTO();
        }

        public async Task<bool> DeleteClassTeacherAsync(Guid id)
        {
            return await _classTeacherRepository.DeleteAsync(id);
        }

        public async Task<ClassTeacherResponseDTO?> AssignTeacherToClassAsync(Guid teacherId, Guid classId, int tahun)
        {
            var createDto = new CreateClassTeacherDTO
            {
                IdTeacher = teacherId,
                IdClass = classId,
                Tahun = tahun,
                Status = AssignmentStatus.Assigned
            };

            return await CreateClassTeacherAsync(createDto);
        }

        public async Task<bool> UnassignTeacherFromClassAsync(Guid id)
        {
            var classTeacher = await _classTeacherRepository.GetByIdAsync(id);
            if (classTeacher == null) return false;

            var updateDto = new CreateClassTeacherDTO
            {
                IdTeacher = classTeacher.IdTeacher,
                IdClass = classTeacher.IdClass,
                Tahun = classTeacher.Tahun,
                Status = AssignmentStatus.Unassigned
            };

            var result = await UpdateClassTeacherAsync(id, updateDto);
            return result != null;
        }

        public async Task<bool> AssignTeacherFromClassAsync(Guid id)
        {
            var classTeacher = await _classTeacherRepository.GetByIdAsync(id);
            if (classTeacher == null) return false;

            var updateDto = new CreateClassTeacherDTO
            {
                IdTeacher = classTeacher.IdTeacher,
                IdClass = classTeacher.IdClass,
                Tahun = classTeacher.Tahun,
                Status = AssignmentStatus.Assigned
            };

            var result = await UpdateClassTeacherAsync(id, updateDto);
            return result != null;
        }
    }
}
