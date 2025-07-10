using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Students.Dtos;
using SchoolManagementSystem.Modules.Students.Services;

namespace SchoolManagementSystem.Modules.Students
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StudentResponseDTO>> CreateStudent([FromBody] CreateStudentDTO createStudentDto)
        {
            try
            {
                var student = await _studentService.CreateStudentAsync(createStudentDto);
                return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the student.", details = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<StudentResponseDTO>> GetStudentById(Guid id)
        {
            try
            {
                var student = await _studentService.GetStudentByIdAsync(id);
                if (student == null)
                {
                    return NotFound(new { message = "Student not found." });
                }
                return Ok(student);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the student.", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentResponseDTO>>> GetAllStudents()
        {
            try
            {
                var students = await _studentService.GetAllStudentsAsync();
                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving students.", details = ex.Message });
            }
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StudentResponseDTO>> UpdateStudent(Guid id, [FromBody] CreateStudentDTO updateStudentDto)
        {
            try
            {
                var student = await _studentService.UpdateStudentAsync(id, updateStudentDto);
                if (student == null)
                {
                    return NotFound(new { message = "Student not found." });
                }
                return Ok(student);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the student.", details = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteStudent(Guid id)
        {
            try
            {
                var result = await _studentService.DeleteStudentAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Student not found." });
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
