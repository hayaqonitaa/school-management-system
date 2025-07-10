using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Modules.Teachers.Dtos;
using SchoolManagementSystem.Modules.Teachers.Entities;
using SchoolManagementSystem.Modules.Users.Entities;
using SchoolManagementSystem.Common.Constants;
using SchoolManagementSystem.Common.Models;

namespace SchoolManagementSystem.Modules.Teachers.Services
{
    public interface ITeacherService
    {
        Task<TeacherResponseDTO> CreateTeacherAsync(CreateTeacherDTO createTeacherDTO);
        Task<TeacherResponseDTO?> GetTeacherByIdAsync(Guid id);
        Task<List<TeacherResponseDTO>> GetAllTeachersAsync();
        Task<(List<TeacherResponseDTO> teachers, int totalCount)> GetAllTeachersPaginatedAsync(int page, int pageSize);
        Task<TeacherResponseDTO> UpdateTeacherAsync(Guid id, CreateTeacherDTO updateTeacherDTO);
        Task<bool> DeleteTeacherAsync(Guid id);
    }
}
