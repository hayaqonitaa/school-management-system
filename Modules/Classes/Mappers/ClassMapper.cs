using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Entities;

namespace SchoolManagementSystem.Modules.Classes.Mappers
{
    public static class ClassMapper
    {
        // DTO → Entity
        public static Class ToEntity(this CreateClassDTO dto)
        {
            return new Class
            {
                Kelas = dto.Kelas,
                Tingkat = dto.Tingkat
            };
        }

        // Entity → Response DTO
        public static ClassResponseDTO ToResponseDTO(this Class entity)
        {
            return new ClassResponseDTO
            {
                Id = entity.Id,
                Kelas = entity.Kelas,
                Tingkat = entity.Tingkat,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        // List Entity → List Response DTO
        public static List<ClassResponseDTO> ToResponseDTOList(this List<Class> entities)
        {
            return entities.Select(e => e.ToResponseDTO()).ToList();
        }

        // Update Entity from DTO
        public static void UpdateFromDTO(this Class entity, CreateClassDTO dto)
        {
            entity.Kelas = dto.Kelas;
            entity.Tingkat = dto.Tingkat;
        }
    }
}
