using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Services;
using System.Security.Claims;

namespace SchoolManagementSystem.Modules.Classes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassesController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<List<ClassResponseDTO>>> GetAllClasses()
        {
            try
            {
                var classes = await _classService.GetAllClassesAsync();
                return Ok(classes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving classes.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<ClassResponseDTO>> GetClassById(Guid id)
        {
            try
            {
                var classEntity = await _classService.GetClassByIdAsync(id);
                if (classEntity == null)
                    return NotFound(new { message = "Class not found" });

                return Ok(classEntity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the class.", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClassResponseDTO>> CreateClass([FromBody] CreateClassDTO createClassDto)
        {
            try
            {
                var classEntity = await _classService.CreateClassAsync(createClassDto);
                return CreatedAtAction(nameof(GetClassById), new { id = classEntity.Id }, classEntity);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the class.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClassResponseDTO>> UpdateClass(Guid id, [FromBody] CreateClassDTO updateClassDto)
        {
            try
            {
                var classEntity = await _classService.UpdateClassAsync(id, updateClassDto);
                if (classEntity == null)
                    return NotFound(new { message = "Class not found" });

                return Ok(classEntity);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the class.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteClass(Guid id)
        {
            try
            {
                var result = await _classService.DeleteClassAsync(id);
                if (!result)
                    return NotFound(new { message = "Class not found" });

                return Ok(new { message = "Class deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the class.", details = ex.Message });
            }
        }
    }
}
