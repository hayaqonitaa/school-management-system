using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Students.Dtos;
using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Modules.Students.Mappers;
using SchoolManagementSystem.Modules.Users.Entities;
using SchoolManagementSystem.Common.Constants;

namespace SchoolManagementSystem.Modules.Students.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly DatabaseConfig _context;
        
        public StudentService(IStudentRepository studentRepository, DatabaseConfig context)
        {
            _studentRepository = studentRepository;
            _context = context;
        }
        
        public async Task<StudentResponseDTO> CreateStudentAsync(CreateStudentDTO createStudentDTO)
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
        
        public async Task<StudentResponseDTO?> GetStudentByIdAsync(Guid id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null) return null;
            
            return student.ToResponseDTO();
        }
        
        public async Task<List<StudentResponseDTO>> GetAllStudentsAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return students.ToResponseDTOList();
        }
        
        public async Task<StudentResponseDTO> UpdateStudentAsync(Guid id, CreateStudentDTO updateStudentDTO)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
            {
                throw new ArgumentException("Student not found");
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
        
        public async Task<bool> DeleteStudentAsync(Guid id)
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
    }
}
