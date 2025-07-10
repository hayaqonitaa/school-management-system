using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Services;
using SchoolManagementSystem.Modules.Classes.Entities;

namespace SchoolManagementSystem.Modules.Classes.Controllers
{
    [Route("api/classteachers")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ClassTeachersController : ControllerBase
    {
        private readonly IClassTeacherService _classTeacherService;

        public ClassTeachersController(IClassTeacherService classTeacherService)
        {
            _classTeacherService = classTeacherService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ClassTeacherResponseDTO>>> GetAllClassTeachers()
        {
            try
            {
                var classTeachers = await _classTeacherService.GetAllClassTeachersAsync();
                return Ok(classTeachers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving class teachers.", details = ex.Message });
            }
        }

       

        [HttpPost]
        public async Task<ActionResult<ClassTeacherResponseDTO>> AssignTeacherToClass([FromBody] AssignTeacherRequestDTO assignRequest)
        {
            try
            {
                var classTeacher = await _classTeacherService.AssignTeacherToClassAsync(
                    assignRequest.IdTeacher, assignRequest.IdClass, assignRequest.Tahun);
                
                if (classTeacher == null)
                    return BadRequest(new { message = "Failed to assign teacher to class" });

                return Ok(classTeacher);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while assigning teacher to class.", details = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ClassTeacherResponseDTO>> UpdateClassTeacher(Guid id, [FromBody] CreateClassTeacherDTO updateClassTeacherDto)
        {
            try
            {
                var classTeacher = await _classTeacherService.UpdateClassTeacherAsync(id, updateClassTeacherDto);
                if (classTeacher == null)
                    return NotFound(new { message = "Class teacher assignment not found" });

                return Ok(classTeacher);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the class teacher assignment.", details = ex.Message });
            }
        }

        [HttpPut("{id}/unassign")]
        public async Task<ActionResult> UnassignTeacherFromClass(Guid id)
        {
            try
            {
                var result = await _classTeacherService.UnassignTeacherFromClassAsync(id);
                if (!result)
                    return NotFound(new { message = "Class teacher assignment not found" });

                return Ok(new { message = "Teacher unassigned successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while unassigning teacher from class.", details = ex.Message });
            }
        }

        [HttpPut("{id}/assign")]
        public async Task<ActionResult> AssignTeacherFromClass(Guid id)
        {
            try
            {
                var result = await _classTeacherService.AssignTeacherFromClassAsync(id);
                if (!result)
                    return NotFound(new { message = "Class teacher assignment not found" });

                return Ok(new { message = "Teacher assigned successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while assigning teacher to class.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClassTeacher(Guid id)
        {
            try
            {
                var result = await _classTeacherService.DeleteClassTeacherAsync(id);
                if (!result)
                    return NotFound(new { message = "Class teacher assignment not found" });

                return Ok(new { message = "Class teacher assignment deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the class teacher assignment.", details = ex.Message });
            }
        }
    }

    // Helper DTO for assign endpoint
    public class AssignTeacherRequestDTO
    {
        public Guid IdTeacher { get; set; }
        public Guid IdClass { get; set; }
        public int Tahun { get; set; }
    }
}
