using BCrypt.Net;
using SchoolManagementSystem.Modules.Students.Dtos;
using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Students.Repositories;

namespace SchoolManagementSystem.Modules.Students.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        
        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }
        
        public async Task<StudentResponseDTO> CreateStudentAsync(CreateStudentDTO createStudentDTO)
        {
            // cek NISN
            var existingStudentByNISN = await _studentRepository.GetByNISNAsync(createStudentDTO.NISN);
            if (existingStudentByNISN != null)
            {
                throw new ArgumentException("NISN already exists");
            }
            
            // cek email
            var existingStudentByEmail = await _studentRepository.GetByEmailAsync(createStudentDTO.Email);
            if (existingStudentByEmail != null)
            {
                throw new ArgumentException("Email already exists");
            }
            
            // hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createStudentDTO.Password);
            
            // create student entity
            var student = new Student
            {
                NISN = createStudentDTO.NISN,
                FullName = createStudentDTO.FullName,
                Email = createStudentDTO.Email,
                Password = hashedPassword,
                PhoneNumber = createStudentDTO.PhoneNumber
            };
            
            // save to database
            var createdStudent = await _studentRepository.CreateAsync(student);
            
            // return response DTO
            return new StudentResponseDTO
            {
                Id = createdStudent.Id,
                NISN = createdStudent.NISN,
                FullName = createdStudent.FullName,
                Email = createdStudent.Email,
                PhoneNumber = createdStudent.PhoneNumber
            };
        }
        
        public async Task<StudentResponseDTO?> GetStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null) return null;
            
            return new StudentResponseDTO
            {
                Id = student.Id,
                NISN = student.NISN,
                FullName = student.FullName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
            };
        }
        
        public async Task<List<StudentResponseDTO>> GetAllStudentsAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            
            return students.Select(student => new StudentResponseDTO
            {
                Id = student.Id,
                NISN = student.NISN,
                FullName = student.FullName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber
            }).ToList();
        }
        
        public async Task<StudentResponseDTO> UpdateStudentAsync(int id, CreateStudentDTO updateStudentDTO)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
            {
                throw new ArgumentException("Student not found");
            }
            
            var existingStudentByNISN = await _studentRepository.GetByNISNAsync(updateStudentDTO.NISN);
            if (existingStudentByNISN != null && existingStudentByNISN.Id != id)
            {
                throw new ArgumentException("NISN already exists");
            }
            
            // if email already exists (kecuali current student)
            var existingStudentByEmail = await _studentRepository.GetByEmailAsync(updateStudentDTO.Email);
            if (existingStudentByEmail != null && existingStudentByEmail.Id != id)
            {
                throw new ArgumentException("Email already exists");
            }
            
            // update student properties
            student.NISN = updateStudentDTO.NISN;
            student.FullName = updateStudentDTO.FullName;
            student.Email = updateStudentDTO.Email;
            student.PhoneNumber = updateStudentDTO.PhoneNumber;
            
            // update password
            if (!string.IsNullOrEmpty(updateStudentDTO.Password))
            {
                student.Password = BCrypt.Net.BCrypt.HashPassword(updateStudentDTO.Password);
            }
            
            var updatedStudent = await _studentRepository.UpdateAsync(student);
            
            return new StudentResponseDTO
            {
                Id = updatedStudent.Id,
                NISN = updatedStudent.NISN,
                FullName = updatedStudent.FullName,
                Email = updatedStudent.Email,
                PhoneNumber = updatedStudent.PhoneNumber,
            };
        }
        
        public async Task<bool> DeleteStudentAsync(int id)
        {
            return await _studentRepository.DeleteAsync(id);
        }
    }
}
