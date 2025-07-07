using SchoolManagementSystem.Modules.Students.Entities;
using SchoolManagementSystem.Modules.Teachers.Dtos;
using SchoolManagementSystem.Modules.Teachers.Entities;

namespace SchoolManagementSystem.Modules.Teachers.Mappers
{
    public static class TeacherMapper
    {
        public static Teacher ToEntity(this CreateTeacherDTO dto, string hashedPassword)
        {
            return new Teacher
            {
                NIP = dto.NIP,
                FullName = dto.FullName,
                Alamat = dto.Alamat,
                Email = dto.Email,
                Password = hashedPassword,
                PhoneNumber = dto.PhoneNumber
            };
        }

        public static TeacherResponseDTO ToResponseDTO(this Teacher entity)
        {
            return new TeacherResponseDTO
            {
                Id = entity.Id,
                NIP = entity.NIP,
                FullName = entity.FullName,
                Alamat = entity.Alamat,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static List<TeacherResponseDTO> ToResponseDTOList(this List<Teacher> entities)
        {
            return entities.Select(e => e.ToResponseDTO()).ToList();
        }

        public static void UpdateFromDTO(this Teacher entity, CreateTeacherDTO dto, string? hashedPassword = null)
        {
            entity.NIP = dto.NIP;
            entity.FullName = dto.FullName;
            entity.Alamat = dto.Alamat;
            entity.Email = dto.Email;
            entity.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrEmpty(hashedPassword))
            {
                entity.Password = hashedPassword;
            }
        }
    }
}