using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Modules.Enrollments.Services;

namespace SchoolManagementSystem.Modules.Enrollments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // GET: api/enrollments
        [HttpGet]
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

        // GET: api/enrollments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentResponseDTO>> GetEnrollmentById(Guid id)
        {
            try
            {
                var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
                if (enrollment == null)
                    return NotFound(new { message = "Enrollment not found" });

                return Ok(enrollment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the enrollment.", details = ex.Message });
            }
        }

        // GET: api/enrollments/student/{studentId}
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<List<EnrollmentResponseDTO>>> GetEnrollmentsByStudentId(Guid studentId)
        {
            try
            {
                var enrollments = await _enrollmentService.GetEnrollmentsByStudentIdAsync(studentId);
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving enrollments by student.", details = ex.Message });
            }
        }

        // GET: api/enrollments/classteacher/{classTeacherId}
        [HttpGet("classteacher/{classTeacherId}")]
        public async Task<ActionResult<List<EnrollmentResponseDTO>>> GetEnrollmentsByClassTeacherId(Guid classTeacherId)
        {
            try
            {
                var enrollments = await _enrollmentService.GetEnrollmentsByClassTeacherIdAsync(classTeacherId);
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving enrollments by class teacher.", details = ex.Message });
            }
        }

        // POST: api/enrollments
        [HttpPost]
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

        // PUT: api/enrollments/{id}
        [HttpPut("{id}")]
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

        // DELETE: api/enrollments/{id}
        [HttpDelete("{id}")]
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

    // Helper DTO for enroll endpoint
    public class EnrollStudentRequestDTO
    {
        public Guid IdStudent { get; set; }
        public Guid IdClassTeacher { get; set; }
    }
}
