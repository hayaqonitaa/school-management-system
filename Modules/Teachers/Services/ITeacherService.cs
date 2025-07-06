using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Teachers.Dtos;
using SchoolManagementSystem.Modules.Teachers.Entities;
using SchoolManagementSystem.Modules.Users.Entities;
using SchoolManagementSystem.Common.Constants;

namespace SchoolManagementSystem.Modules.Teachers.Services
{
    public interface ITeacherService
    {
        Task<TeacherResponseDTO> CreateTeacherAsync(CreateTeacherDTO createTeacherDTO);
        Task<TeacherResponseDTO?> GetTeacherByIdAsync(Guid id);
        Task<TeacherResponseDTO?> GetTeacherByNIPAsync(string nip);
        Task<TeacherResponseDTO?> GetTeacherByEmailAsync(string email);
        Task<List<TeacherResponseDTO>> GetAllTeachersAsync();
        Task<TeacherResponseDTO> UpdateTeacherAsync(Guid id, CreateTeacherDTO updateTeacherDTO);
        Task<bool> DeleteTeacherAsync(Guid id);
    }
}
