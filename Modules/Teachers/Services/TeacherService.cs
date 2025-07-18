using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Common.Constants;
using SchoolManagementSystem.Modules.Teachers.Dtos;
using SchoolManagementSystem.Modules.Teachers.Entities;
using SchoolManagementSystem.Modules.Teachers.Repositories;
using SchoolManagementSystem.Modules.Users.Entities;
using SchoolManagementSystem.Modules.Teachers.Mappers;
using SchoolManagementSystem.Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Modules.Teachers.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepostory;
        private readonly DatabaseConfig _context;
        private readonly ILogger<TeacherService> _logger;

        public TeacherService(ITeacherRepository teacherRepostory, DatabaseConfig context, ILogger<TeacherService> logger)
        {
            _teacherRepostory = teacherRepostory;
            _context = context;
            _logger = logger;
        }

        public async Task<TeacherResponseDTO> CreateTeacherAsync(CreateTeacherDTO createTeacherDTO)
        {
            try
            {
                var existingTeacherByNIP = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.NIP == createTeacherDTO.NIP);
                if (existingTeacherByNIP != null)
                {
                    throw new ArgumentException("NIP already exists");
                }

                var existingTeacherByEmail = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.Email == createTeacherDTO.Email);
                if (existingTeacherByEmail != null)
                {
                    throw new ArgumentException("Email already exists");
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createTeacherDTO.Password);

                var teacher = createTeacherDTO.ToEntity(hashedPassword);

                var createdTeacher = await _teacherRepostory.CreateAsync(teacher);

                var user = new User
                {
                    IdUser = createdTeacher.Id,
                    Role = UserRoles.Teacher
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return createdTeacher.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating teacher: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<TeacherResponseDTO?> GetTeacherByIdAsync(Guid id)
        {
            try 
            {
                var teacher = await _teacherRepostory.GetByIdAsync(id);
                return teacher?.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teacher by ID {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<TeacherResponseDTO>> GetAllTeachersAsync()
        {
            try 
            {
                var teachers = await _teacherRepostory.GetAllAsync();
                return teachers.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all teachers: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<(List<TeacherResponseDTO> teachers, int totalCount)> GetAllTeachersPaginatedAsync(int page, int pageSize)
        {
            try 
            {
                var (teachers, totalCount) = await _teacherRepostory.GetAllPaginatedAsync(page, pageSize);
                var teacherDTOs = teachers.ToResponseDTOList();
                return (teacherDTOs, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated teachers (page: {Page}, size: {Size}): {Message}", page, pageSize, ex.Message);
                throw;
            }
        }

        public async Task<TeacherResponseDTO> UpdateTeacherAsync(Guid id, CreateTeacherDTO updateTeacherDTO)
        {
            try
            {
                var teacher = await _teacherRepostory.GetByIdAsync(id);
                if (teacher == null)
                {
                    throw new KeyNotFoundException("Teacher not found");
                }
                var existingTeacherByNIP = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.NIP == updateTeacherDTO.NIP && t.Id != id);
                if (existingTeacherByNIP != null)
                {
                    throw new ArgumentException("NIP already exists");
                }

                var existingTeacherByEmail = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.Email == updateTeacherDTO.Email && t.Id != id);
                if (existingTeacherByEmail != null)
                {
                    throw new ArgumentException("Email already exists");
                }

                var hashedPassword = !string.IsNullOrEmpty(updateTeacherDTO.Password)
                    ? BCrypt.Net.BCrypt.HashPassword(updateTeacherDTO.Password)
                    : null;

                teacher.UpdateFromDTO(updateTeacherDTO, hashedPassword);

                var updatedTeacher = await _teacherRepostory.UpdateAsync(teacher);

                return updatedTeacher.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating teacher {Id}: {Message}", id, ex.Message);
                throw;
            }
        }
        
        public async Task<bool> DeleteTeacherAsync(Guid id)
        {
            try
            {
                // delete user record 
                var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == id && u.Role == UserRoles.Teacher);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }

                // delete teacher record
                return await _teacherRepostory.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting teacher {Id}: {Message}", id, ex.Message);
                throw;
            }
        }
    }
}