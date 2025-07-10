using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Repositories;
using SchoolManagementSystem.Modules.Classes.Mappers;
using SchoolManagementSystem.Common.Models;

namespace SchoolManagementSystem.Modules.Classes.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;

        public ClassService(IClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        public async Task<List<ClassResponseDTO>> GetAllClassesAsync()
        {
            var classes = await _classRepository.GetAllAsync();
            return classes.ToResponseDTOList();
        }

        public async Task<(List<ClassResponseDTO> classes, int totalCount)> GetAllClassesPaginatedAsync(int page, int pageSize)
        {
            var (classes, totalCount) = await _classRepository.GetAllPaginatedAsync(page, pageSize);
            var classDTOs = classes.ToResponseDTOList();
            return (classDTOs, totalCount);
        }

        public async Task<ClassResponseDTO?> GetClassByIdAsync(Guid id)
        {
            var classEntity = await _classRepository.GetByIdAsync(id);
            return classEntity?.ToResponseDTO();
        }

        public async Task<List<ClassResponseDTO>> GetClassesByTingkatAsync(int tingkat)
        {
            var classes = await _classRepository.GetByTingkatAsync(tingkat);
            return classes.ToResponseDTOList();
        }

        public async Task<ClassResponseDTO> CreateClassAsync(CreateClassDTO createClassDto)
        {
            // Check if class name already exists
            var existingClass = await _classRepository.GetByKelasAsync(createClassDto.Kelas);
            if (existingClass != null)
                throw new ArgumentException("Class name already exists");

            // Create class
            var classEntity = createClassDto.ToEntity();
            var createdClass = await _classRepository.CreateAsync(classEntity);

            return createdClass.ToResponseDTO();
        }

        public async Task<ClassResponseDTO?> UpdateClassAsync(Guid id, CreateClassDTO updateClassDto)
        {
            var existingClass = await _classRepository.GetByIdAsync(id);
            if (existingClass == null) return null;

            // Check class name uniqueness (exclude current class)
            var classWithName = await _classRepository.GetByKelasAsync(updateClassDto.Kelas);
            if (classWithName != null && classWithName.Id != id)
                throw new ArgumentException("Class name already exists");

            // Update class
            existingClass.UpdateFromDTO(updateClassDto);
            var updatedClass = await _classRepository.UpdateAsync(existingClass);

            return updatedClass.ToResponseDTO();
        }

        public async Task<bool> DeleteClassAsync(Guid id)
        {
            return await _classRepository.DeleteAsync(id);
        }
    }
}
