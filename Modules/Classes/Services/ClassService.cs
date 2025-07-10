using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Repositories;
using SchoolManagementSystem.Modules.Classes.Mappers;
using SchoolManagementSystem.Common.Models;
using Microsoft.Extensions.Logging;

namespace SchoolManagementSystem.Modules.Classes.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly ILogger<ClassService> _logger;

        public ClassService(IClassRepository classRepository, ILogger<ClassService> logger)
        {
            _classRepository = classRepository;
            _logger = logger;
        }

        public async Task<List<ClassResponseDTO>> GetAllClassesAsync()
        {
            try
            {
                var classes = await _classRepository.GetAllAsync();
                return classes.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all classes: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<(List<ClassResponseDTO> classes, int totalCount)> GetAllClassesPaginatedAsync(int page, int pageSize)
        {
            try
            {
                var (classes, totalCount) = await _classRepository.GetAllPaginatedAsync(page, pageSize);
                var classDTOs = classes.ToResponseDTOList();
                return (classDTOs, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated classes (page: {Page}, size: {Size}): {Message}", page, pageSize, ex.Message);
                throw;
            }
        }

        public async Task<ClassResponseDTO?> GetClassByIdAsync(Guid id)
        {
            try
            {
                var classEntity = await _classRepository.GetByIdAsync(id);
                return classEntity?.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class by ID {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<ClassResponseDTO>> GetClassesByTingkatAsync(int tingkat)
        {
            try
            {
                var classes = await _classRepository.GetByTingkatAsync(tingkat);
                return classes.ToResponseDTOList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving classes by tingkat {Tingkat}: {Message}", tingkat, ex.Message);
                throw;
            }
        }

        public async Task<ClassResponseDTO> CreateClassAsync(CreateClassDTO createClassDto)
        {
            try
            {
                // Check if class name already exists
                var existingClass = await _classRepository.GetByKelasAsync(createClassDto.Kelas);
                if (existingClass != null)
                {
                    _logger.LogWarning("Attempted to create class with existing name: {Kelas}", createClassDto.Kelas);
                    throw new ArgumentException("Class name already exists");
                }

                // Create class
                var classEntity = createClassDto.ToEntity();
                var createdClass = await _classRepository.CreateAsync(classEntity);
                
                _logger.LogInformation("Created new class with ID {Id} and name {Kelas}", createdClass.Id, createdClass.Kelas);
                return createdClass.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating class {Kelas}: {Message}", createClassDto.Kelas, ex.Message);
                throw;
            }
        }

        public async Task<ClassResponseDTO?> UpdateClassAsync(Guid id, CreateClassDTO updateClassDto)
        {
            try
            {
                var existingClass = await _classRepository.GetByIdAsync(id);
                if (existingClass == null)
                {
                    _logger.LogWarning("Attempted to update non-existent class with ID: {Id}", id);
                    return null;
                }

                // Check class name uniqueness (exclude current class)
                var classWithName = await _classRepository.GetByKelasAsync(updateClassDto.Kelas);
                if (classWithName != null && classWithName.Id != id)
                {
                    _logger.LogWarning("Attempted to update class {Id} with already existing name: {Kelas}", id, updateClassDto.Kelas);
                    throw new ArgumentException("Class name already exists");
                }

                // Update class
                existingClass.UpdateFromDTO(updateClassDto);
                var updatedClass = await _classRepository.UpdateAsync(existingClass);
                
                _logger.LogInformation("Updated class with ID {Id} to name {Kelas}", updatedClass.Id, updatedClass.Kelas);
                return updatedClass.ToResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteClassAsync(Guid id)
        {
            try
            {
                var result = await _classRepository.DeleteAsync(id);
                if (result)
                {
                    _logger.LogInformation("Deleted class with ID {Id}", id);
                }
                else
                {
                    _logger.LogWarning("Attempted to delete non-existent class with ID: {Id}", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting class {Id}: {Message}", id, ex.Message);
                throw;
            }
        }
    }
}
