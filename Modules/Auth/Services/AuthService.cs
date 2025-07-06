using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Common.Constants;
using SchoolManagementSystem.Common.Helpers;
using SchoolManagementSystem.Modules.Auth.Dtos;
using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Teachers.Entities;
using SchoolManagementSystem.Modules.Users.Entities;

namespace SchoolManagementSystem.Modules.Auth.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest);
        Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO registerRequest);
    }
    
    public class AuthService : IAuthService
    {
        private readonly DatabaseConfig _context;
        private readonly JwtHelper _jwtHelper;
        
        public AuthService(DatabaseConfig context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }
        
        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest)
        {
            // Find user by email in Students or Teachers
            Student? student = await _context.Students.FirstOrDefaultAsync(s => s.Email == loginRequest.Email);
            Teacher? teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == loginRequest.Email);
            
            if (student == null && teacher == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
            
            // Verify password
            string hashedPassword;
            Guid userId;
            int roleId;
            
            if (student != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, student.Password))
                {
                    throw new UnauthorizedAccessException("Invalid email or password");
                }
                hashedPassword = student.Password;
                userId = student.Id;
                roleId = UserRoles.Student;
            }
            else // teacher != null
            {
                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, teacher!.Password))
                {
                    throw new UnauthorizedAccessException("Invalid email or password");
                }
                hashedPassword = teacher.Password;
                userId = teacher.Id;
                roleId = UserRoles.Teacher;
            }
            
            // Create or update User record
            var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == userId && u.Role == roleId);
            if (user == null)
            {
                user = new User
                {
                    IdUser = userId,
                    Role = roleId
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            
            // Generate JWT token
            var token = _jwtHelper.GenerateToken(user.Id, loginRequest.Email, roleId);
            
            return new LoginResponseDTO
            {
                Token = token,
                UserId = user.Id,
                Email = loginRequest.Email,
                Role = UserRoles.GetRoleName(roleId),
                RoleId = roleId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }
        
        public async Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO registerRequest)
        {
            // Validate role
            if (!UserRoles.IsValidRole(registerRequest.RoleId))
            {
                throw new ArgumentException("Invalid role");
            }
            
            // Check if email already exists
            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.Email == registerRequest.Email);
            var existingTeacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == registerRequest.Email);
            
            if (existingStudent != null || existingTeacher != null)
            {
                throw new ArgumentException("Email already exists");
            }
            
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
            Guid createdId;
            
            // Create Student or Teacher based on role
            if (registerRequest.RoleId == UserRoles.Student)
            {
                if (string.IsNullOrEmpty(registerRequest.NISN) || string.IsNullOrEmpty(registerRequest.FullName))
                {
                    throw new ArgumentException("NISN and FullName are required for Student registration");
                }
                
                var student = new Student
                {
                    NISN = registerRequest.NISN,
                    FullName = registerRequest.FullName,
                    Email = registerRequest.Email,
                    Password = hashedPassword,
                    PhoneNumber = registerRequest.PhoneNumber
                };
                
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                createdId = student.Id;
            }
            else if (registerRequest.RoleId == UserRoles.Teacher)
            {
                if (string.IsNullOrEmpty(registerRequest.NIP) || string.IsNullOrEmpty(registerRequest.FullName))
                {
                    throw new ArgumentException("NIP and FullName are required for Teacher registration");
                }
                
                var teacher = new Teacher
                {
                    NIP = registerRequest.NIP,
                    FullName = registerRequest.FullName,
                    Alamat = registerRequest.Address ?? "", // Fix property name
                    Email = registerRequest.Email,
                    Password = hashedPassword,
                    PhoneNumber = registerRequest.PhoneNumber
                };
                
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
                createdId = teacher.Id;
            }
            else
            {
                throw new ArgumentException("Admin registration is not allowed through this endpoint");
            }
            
            // Create User record
            var user = new User
            {
                IdUser = createdId,
                Role = registerRequest.RoleId
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            // Generate JWT token
            var token = _jwtHelper.GenerateToken(user.Id, registerRequest.Email, registerRequest.RoleId);
            
            return new LoginResponseDTO
            {
                Token = token,
                UserId = user.Id,
                Email = registerRequest.Email,
                Role = UserRoles.GetRoleName(registerRequest.RoleId),
                RoleId = registerRequest.RoleId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }
    }
}
