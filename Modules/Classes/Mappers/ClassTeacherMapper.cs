using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Entities;

namespace SchoolManagementSystem.Modules.Classes.Mappers
{
    public static class ClassTeacherMapper
    {
        // DTO → Entity
        public static ClassTeacher ToEntity(this CreateClassTeacherDTO dto)
        {
            return new ClassTeacher
            {
                IdTeacher = dto.IdTeacher,
                IdClass = dto.IdClass,
                Tahun = dto.Tahun,
                Status = dto.Status
            };
        }

        // Entity → Response DTO
        public static ClassTeacherResponseDTO ToResponseDTO(this ClassTeacher entity)
        {
            return new ClassTeacherResponseDTO
            {
                Id = entity.Id,
                IdTeacher = entity.IdTeacher,
                IdClass = entity.IdClass,
                Tahun = entity.Tahun,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                TeacherName = entity.Teacher?.FullName ?? string.Empty,
                ClassName = entity.Class?.Kelas ?? string.Empty,
                ClassTingkat = entity.Class?.Tingkat ?? 0
            };
        }

        // List Entity → List Response DTO
        public static List<ClassTeacherResponseDTO> ToResponseDTOList(this List<ClassTeacher> entities)
        {
            return entities.Select(e => e.ToResponseDTO()).ToList();
        }

        // Update Entity from DTO
        public static void UpdateFromDTO(this ClassTeacher entity, CreateClassTeacherDTO dto)
        {
            entity.IdTeacher = dto.IdTeacher;
            entity.IdClass = dto.IdClass;
            entity.Tahun = dto.Tahun;
            entity.Status = dto.Status;
        }
    }
}
