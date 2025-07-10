using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Teachers.Dtos;
using SchoolManagementSystem.Modules.Teachers.Services;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementSystem.Modules.Teachers.Controllers
{
    [Route("api/teachers")]
    [ApiController]

    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TeacherResponseDTO>> CreateTeacher([FromBody] CreateTeacherDTO createTeacherDto)
        {
            try
            {
                var teacher = await _teacherService.CreateTeacherAsync(createTeacherDto);
                return CreatedAtAction(nameof(GetTeacherById), new { id = teacher.Id }, teacher);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the teacher.", details = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TeacherResponseDTO>> GetTeacherById(Guid id)
        {
            try
            {
                var teacher = await _teacherService.GetTeacherByIdAsync(id);
                if (teacher == null)
                {
                    return NotFound(new { message = "Teacher not found." });
                }
                return Ok(teacher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the teacher.", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<TeacherResponseDTO>>> GetAllTeachers()
        {
            try
            {
                var teachers = await _teacherService.GetAllTeachersAsync();
                if (teachers == null || teachers.Count == 0)
                {
                    return NotFound(new { message = "No teachers found." });
                }
                return Ok(teachers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the teacher.", details = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TeacherResponseDTO>> UpdateTeacher(Guid id, [FromBody] CreateTeacherDTO updateTeacherDto)
        {
            try
            {
                var updatedTeacher = await _teacherService.UpdateTeacherAsync(id, updateTeacherDto);
                if (updatedTeacher == null)
                {
                    return NotFound(new { message = "Teacher not found." });
                }
                return Ok(updatedTeacher);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the teacher.", details = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTeacher(Guid id)
        {
            try
            {
                var result = await _teacherService.DeleteTeacherAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Teacher not found." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the student.", details = ex.Message });
            }
        }

    }
}