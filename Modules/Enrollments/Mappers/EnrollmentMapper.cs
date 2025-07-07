using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Modules.Enrollments.Entities;

namespace SchoolManagementSystem.Modules.Enrollments.Mappers
{
    public static class EnrollmentMapper
    {
        // DTO → Entity
        public static Enrollment ToEntity(this CreateEnrollmentDTO dto)
        {
            return new Enrollment
            {
                IdStudent = dto.IdStudent,
                IdClassTeacher = dto.IdClassTeacher
            };
        }

        // Entity → Response DTO
        public static EnrollmentResponseDTO ToResponseDTO(this Enrollment entity)
        {
            return new EnrollmentResponseDTO
            {
                Id = entity.Id,
                IdStudent = entity.IdStudent,
                IdClassTeacher = entity.IdClassTeacher,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                StudentName = entity.Student?.FullName ?? string.Empty,
                StudentNISN = entity.Student?.NISN ?? string.Empty,
                TeacherName = entity.ClassTeacher?.Teacher?.FullName ?? string.Empty,
                ClassName = entity.ClassTeacher?.Class?.Kelas ?? string.Empty,
                ClassTingkat = entity.ClassTeacher?.Class?.Tingkat ?? 0,
                Tahun = entity.ClassTeacher?.Tahun ?? 0
            };
        }

        // List Entity → List Response DTO
        public static List<EnrollmentResponseDTO> ToResponseDTOList(this List<Enrollment> entities)
        {
            return entities.Select(e => e.ToResponseDTO()).ToList();
        }

        // Update Entity from DTO
        public static void UpdateFromDTO(this Enrollment entity, CreateEnrollmentDTO dto)
        {
            entity.IdStudent = dto.IdStudent;
            entity.IdClassTeacher = dto.IdClassTeacher;
        }
    }
}
