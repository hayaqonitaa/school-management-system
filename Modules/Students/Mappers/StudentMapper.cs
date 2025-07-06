using SchoolManagementSystem.Modules.Students.Dtos;
using SchoolManagementSystem.Modules.Students.Entities;

namespace SchoolManagementSystem.Modules.Students.Mappers
{
    public static class StudentMapper
    {
        // DTO → Entity
        public static Student ToEntity(this CreateStudentDTO dto, string hashedPassword)
        {
            return new Student
            {
                NISN = dto.NISN,
                FullName = dto.FullName,
                Email = dto.Email,
                Password = hashedPassword,
                PhoneNumber = dto.PhoneNumber
            };
        }

        // Entity → Response DTO
        public static StudentResponseDTO ToResponseDTO(this Student entity)
        {
            return new StudentResponseDTO
            {
                Id = entity.Id,
                NISN = entity.NISN,
                FullName = entity.FullName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        // List Entity → List Response DTO
        public static List<StudentResponseDTO> ToResponseDTOList(this List<Student> entities)
        {
            return entities.Select(e => e.ToResponseDTO()).ToList();
        }

        // Update Entity from DTO (menyimpan existing properties)
        public static void UpdateFromDTO(this Student entity, CreateStudentDTO dto, string? hashedPassword = null)
        {
            entity.NISN = dto.NISN;
            entity.FullName = dto.FullName;
            entity.Email = dto.Email;
            entity.PhoneNumber = dto.PhoneNumber;
            
            // update password hanya jika diubah
            if (!string.IsNullOrEmpty(hashedPassword))
            {
                entity.Password = hashedPassword;
            }
        }
    }
}
