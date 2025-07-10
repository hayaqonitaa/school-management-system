using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Entities;
using SchoolManagementSystem.Modules.Classes.Repositories;
using SchoolManagementSystem.Modules.Classes.Mappers;
using SchoolManagementSystem.Modules.Teachers.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Modules.Classes.Services
{
    public class ClassTeacherService : IClassTeacherService
    {
        private readonly IClassTeacherRepository _classTeacherRepository;
        private readonly IClassRepository _classRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ILogger<ClassTeacherService> _logger;

        public ClassTeacherService(
            IClassTeacherRepository classTeacherRepository,
            IClassRepository classRepository,
            ITeacherRepository teacherRepository,
            ILogger<ClassTeacherService> logger)
        {
            _classTeacherRepository = classTeacherRepository;
            _classRepository = classRepository;
            _teacherRepository = teacherRepository;
            _logger = logger;
        }

        public async Task<List<ClassTeacherResponseDTO>> GetAllClassTeachersAsync()
        {
            try
            {
                var classTeachers = await _classTeacherRepository.GetAllAsync();
                return classTeachers.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all class teachers: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<(List<ClassTeacherResponseDTO> classTeachers, int totalCount)> GetAllClassTeachersPaginatedAsync(int page, int pageSize)
        {
            try
            {
                var (classTeachers, totalCount) = await _classTeacherRepository.GetAllPaginatedAsync(page, pageSize);
                var classTeacherDTOs = classTeachers.ToResponseDTOList();
                return (classTeacherDTOs, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated class teachers (page: {Page}, size: {Size}): {Message}", page, pageSize, ex.Message);
                throw;
            }
        }

        public async Task<ClassTeacherResponseDTO?> GetClassTeacherByIdAsync(Guid id)
        {
            try
            {
                var classTeacher = await _classTeacherRepository.GetByIdAsync(id);
                return classTeacher?.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class teacher by ID {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<ClassTeacherResponseDTO>> GetByTeacherIdAsync(Guid teacherId)
        {
            try
            {
                var classTeachers = await _classTeacherRepository.GetByTeacherIdAsync(teacherId);
                return classTeachers.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class teachers by teacher ID {TeacherId}: {Message}", teacherId, ex.Message);
                throw;
            }
        }

        public async Task<List<ClassTeacherResponseDTO>> GetByClassIdAsync(Guid classId)
        {
            try
            {
                var classTeachers = await _classTeacherRepository.GetByClassIdAsync(classId);
                return classTeachers.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class teachers by class ID {ClassId}: {Message}", classId, ex.Message);
                throw;
            }
        }

        public async Task<List<ClassTeacherResponseDTO>> GetByTahunAsync(int tahun)
        {
            try
            {
                var classTeachers = await _classTeacherRepository.GetByTahunAsync(tahun);
                return classTeachers.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class teachers by year {Tahun}: {Message}", tahun, ex.Message);
                throw;
            }
        }

        public async Task<List<ClassTeacherResponseDTO>> GetByStatusAsync(AssignmentStatus status)
        {
            try
            {
                var classTeachers = await _classTeacherRepository.GetByStatusAsync(status);
                return classTeachers.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class teachers by status {Status}: {Message}", status, ex.Message);
                throw;
            }
        }

        public async Task<ClassTeacherResponseDTO> CreateClassTeacherAsync(CreateClassTeacherDTO createClassTeacherDto)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating class teacher assignment: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<ClassTeacherResponseDTO?> UpdateClassTeacherAsync(Guid id, CreateClassTeacherDTO updateClassTeacherDto)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class teacher assignment {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteClassTeacherAsync(Guid id)
        {
            try
            {
                return await _classTeacherRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting class teacher assignment {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<ClassTeacherResponseDTO?> AssignTeacherToClassAsync(Guid teacherId, Guid classId, int tahun)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning teacher {TeacherId} to class {ClassId}: {Message}", teacherId, classId, ex.Message);
                throw;
            }
        }

        public async Task<bool> UnassignTeacherFromClassAsync(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unassigning teacher from class {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<bool> AssignTeacherFromClassAsync(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning teacher to class {Id}: {Message}", id, ex.Message);
                throw;
            }
        }
    }
}
