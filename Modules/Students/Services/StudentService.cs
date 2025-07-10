using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Students.Dtos;
using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Modules.Students.Mappers;
using SchoolManagementSystem.Modules.Users.Entities;
using SchoolManagementSystem.Common.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SchoolManagementSystem.Modules.Students.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly DatabaseConfig _context;
        private readonly ILogger<StudentService> _logger;
        
        public StudentService(IStudentRepository studentRepository, DatabaseConfig context, ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository;
            _context = context;
            _logger = logger;
        }
        
        public async Task<StudentResponseDTO> CreateStudentAsync(CreateStudentDTO createStudentDTO)
        {
            try
            {
                // cek NISN
                var existingStudentByNISN = await _context.Students
                    .FirstOrDefaultAsync(s => s.NISN == createStudentDTO.NISN);
                if (existingStudentByNISN != null)
                {
                    throw new ArgumentException("NISN already exists");
                }
                
                // cek email
                var existingStudentByEmail = await _context.Students
                    .FirstOrDefaultAsync(s => s.Email == createStudentDTO.Email);
                if (existingStudentByEmail != null)
                {
                    throw new ArgumentException("Email already exists");
                }
                
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createStudentDTO.Password);
                
                var student = createStudentDTO.ToEntity(hashedPassword);
                
                // save to database
                var createdStudent = await _studentRepository.CreateAsync(student);
                
                // create user record for authentication
                var user = new User
                {
                    IdUser = createdStudent.Id,
                    Role = UserRoles.Student
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                // return response DTO using mapper
                return createdStudent.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student: {Message}", ex.Message);
                throw; // Re-throw to be caught by the global exception filter
            }
        }
        
        public async Task<StudentResponseDTO?> GetStudentByIdAsync(Guid id)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id);
                if (student == null) return null;
                
                return student.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student by ID {Id}: {Message}", id, ex.Message);
                throw;
            }
        }
        
        public async Task<List<StudentResponseDTO>> GetAllStudentsAsync()
        {
            try
            {
                var students = await _studentRepository.GetAllAsync();
                return students.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all students: {Message}", ex.Message);
                throw;
            }
        }
        
        public async Task<StudentResponseDTO> UpdateStudentAsync(Guid id, CreateStudentDTO updateStudentDTO)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id);
                if (student == null)
                {
                    throw new KeyNotFoundException("Student not found");
                }
                
                var existingStudentByNISN = await _context.Students
                    .FirstOrDefaultAsync(s => s.NISN == updateStudentDTO.NISN && s.Id != id);
                if (existingStudentByNISN != null)
                {
                    throw new ArgumentException("NISN already exists");
                }
                
                // if email already exists (kecuali current student)
                var existingStudentByEmail = await _context.Students
                    .FirstOrDefaultAsync(s => s.Email == updateStudentDTO.Email && s.Id != id);
                if (existingStudentByEmail != null)
                {
                    throw new ArgumentException("Email already exists");
                }
                
                // update student properties using mapper
                var hashedPassword = !string.IsNullOrEmpty(updateStudentDTO.Password) 
                    ? BCrypt.Net.BCrypt.HashPassword(updateStudentDTO.Password) 
                    : null;
                
                student.UpdateFromDTO(updateStudentDTO, hashedPassword);
                
                var updatedStudent = await _studentRepository.UpdateAsync(student);
                
                return updatedStudent.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student {Id}: {Message}", id, ex.Message);
                throw;
            }
        }
        
        public async Task<bool> DeleteStudentAsync(Guid id)
        {
            try
            {
                // delete user record 
                var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == id && u.Role == UserRoles.Student);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
                
                // delete student record
                return await _studentRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<(List<StudentResponseDTO> students, int totalCount)> GetAllStudentsPaginatedAsync(int page, int size)
        {
            try
            {
                var (students, totalCount) = await _studentRepository.GetAllPaginatedAsync(page, size);
                var studentDTOs = students.ToResponseDTOList();
                return (studentDTOs, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated students (page: {Page}, size: {Size}): {Message}", page, size, ex.Message);
                throw;
            }
        }
    }
}
