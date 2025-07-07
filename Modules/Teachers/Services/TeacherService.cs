using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Common.Constants;
using SchoolManagementSystem.Modules.Teachers.Dtos;
using SchoolManagementSystem.Modules.Teachers.Entities;
using SchoolManagementSystem.Modules.Teachers.Repositories;
using SchoolManagementSystem.Modules.Users.Entities;
using SchoolManagementSystem.Modules.Teachers.Mappers;

namespace SchoolManagementSystem.Modules.Teachers.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepostory;
        private readonly DatabaseConfig _context;

        public TeacherService(ITeacherRepository teacherRepostory, DatabaseConfig context)
        {
            _teacherRepostory = teacherRepostory;
            _context = context;
        }

        public async Task<TeacherResponseDTO> CreateTeacherAsync(CreateTeacherDTO createTeacherDTO)
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

        public async Task<TeacherResponseDTO?> GetTeacherByIdAsync(Guid id)
        {
            var teacher = await _teacherRepostory.GetByIdAsync(id);
            return teacher?.ToResponseDTO();
        }

        public async Task<List<TeacherResponseDTO>> GetAllTeachersAsync()
        {
            var teachers = await _teacherRepostory.GetAllAsync();
            return teachers.ToResponseDTOList();
        }

        public async Task<TeacherResponseDTO> UpdateTeacherAsync(Guid id, CreateTeacherDTO updateTeacherDTO)
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
        
        public async Task<bool> DeleteTeacherAsync(Guid id)
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
    }
}