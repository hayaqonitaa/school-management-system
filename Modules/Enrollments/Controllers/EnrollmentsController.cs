using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Modules.Enrollments.Services;
using System.Security.Claims;

namespace SchoolManagementSystem.Modules.Enrollments.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<EnrollmentResponseDTO>>> GetAllEnrollments()
        {
            try
            {
                var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving enrollments.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentResponseDTO>> GetEnrollmentById(Guid id)
        {
            try
            {
                var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
                if (enrollment == null)
                    return NotFound(new { message = "Enrollment not found" });

                // cek hak akses
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (currentUserRole == "Student")
                {
                    if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out int userId))
                    {
                        return BadRequest(new { message = "Invalid user ID" });
                    }
                    
                    var studentId = await _enrollmentService.GetStudentIdFromUserIdAsync(userId);
                    if (studentId == null || enrollment.IdStudent != studentId)
                        return Forbid("Students can only view their own enrollment");
                }
                else if (currentUserRole == "Teacher")
                {
                    if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out int userId))
                    {
                        return BadRequest(new { message = "Invalid user ID" });
                    }
                    
                    var teacherId = await _enrollmentService.GetTeacherIdFromUserIdAsync(userId);
                    if (teacherId == null)
                        return BadRequest(new { message = "Teacher not found" });
                        
                    var hasAccess = await _enrollmentService.IsTeacherAuthorizedForEnrollmentAsync(teacherId.Value, id);
                    if (!hasAccess)
                        return Forbid("Teachers can only view enrollments for their assigned classes");
                }

                return Ok(enrollment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the enrollment.", details = ex.Message });
            }
        }

        [HttpGet("my-classes")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<List<EnrollmentResponseDTO>>> GetMyClassEnrollments()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }
                
                var enrollments = await _enrollmentService.GetEnrollmentsByUserIdAsync(userId, currentUserRole!);
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving your class enrollments.", details = ex.Message });
            }
        }

        [HttpGet("my-enrollment")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<List<EnrollmentResponseDTO>>> GetMyEnrollment()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }
                
                var enrollments = await _enrollmentService.GetEnrollmentsByUserIdAsync(userId, currentUserRole!);
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving your enrollment.", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult<EnrollmentResponseDTO>> CreateEnrollment([FromBody] CreateEnrollmentDTO createEnrollmentDto)
        {
            try
            {
                var enrollment = await _enrollmentService.CreateEnrollmentAsync(createEnrollmentDto);
                return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.Id }, enrollment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the enrollment.", details = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult<EnrollmentResponseDTO>> UpdateEnrollment(Guid id, [FromBody] CreateEnrollmentDTO updateEnrollmentDto)
        {
            try
            {
                var enrollment = await _enrollmentService.UpdateEnrollmentAsync(id, updateEnrollmentDto);
                if (enrollment == null)
                    return NotFound(new { message = "Enrollment not found" });

                return Ok(enrollment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the enrollment.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteEnrollment(Guid id)
        {
            try
            {
                var result = await _enrollmentService.DeleteEnrollmentAsync(id);
                if (!result)
                    return NotFound(new { message = "Enrollment not found" });

                return Ok(new { message = "Enrollment deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the enrollment.", details = ex.Message });
            }
        }
    }
}
