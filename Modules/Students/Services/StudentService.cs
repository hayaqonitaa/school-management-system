using BCrypt.Net;
using SchoolManagementSystem.Modules.Students.Dtos;
using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Students.Repositories;
using SchoolManagementSystem.Modules.Students.Mappers;

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
            
            // create student entity using mapper
            var student = createStudentDTO.ToEntity(hashedPassword);
            
            // save to database
            var createdStudent = await _studentRepository.CreateAsync(student);
            
            // return response DTO using mapper
            return createdStudent.ToResponseDTO();
        }
        
        public async Task<StudentResponseDTO?> GetStudentByIdAsync(int id)
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
            
            // update student properties using mapper
            var hashedPassword = !string.IsNullOrEmpty(updateStudentDTO.Password) 
                ? BCrypt.Net.BCrypt.HashPassword(updateStudentDTO.Password) 
                : null;
            
            student.UpdateFromDTO(updateStudentDTO, hashedPassword);
            
            var updatedStudent = await _studentRepository.UpdateAsync(student);
            
            return updatedStudent.ToResponseDTO();
        }
        
        public async Task<bool> DeleteStudentAsync(int id)
        {
            return await _studentRepository.DeleteAsync(id);
        }
    }
}
