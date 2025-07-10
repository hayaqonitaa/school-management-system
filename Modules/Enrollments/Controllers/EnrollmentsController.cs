using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Enrollments.Dtos;
using SchoolManagementSystem.Modules.Enrollments.Services;
using System.Security.Claims;
using SchoolManagementSystem.Common.Models;
using SchoolManagementSystem.Common.Helpers;

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
        public async Task<ActionResult<ApiResponse<List<EnrollmentResponseDTO>>>> GetAllEnrollments()
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            var response = ApiResponseHelper.Success(enrollments, $"{enrollments.Count} enrollments retrieved successfully.");
            return Ok(response);
        }

        [HttpGet("paginated")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<EnrollmentResponseDTO>>>> GetAllEnrollmentsPaginated(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var (enrollments, totalCount) = await _enrollmentService.GetAllEnrollmentsPaginatedAsync(page, pageSize);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var paginationMetadata = new PaginationMetadata
            {
                Page = page,
                Size = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPrevious = page > 1,
                HasNext = page < totalPages
            };

            var response = new ApiResponse<List<EnrollmentResponseDTO>>
            {
                Success = true,
                Message = $"{enrollments.Count} enrollments retrieved successfully.",
                Data = enrollments,
                StatusCode = 200,
                Pagination = paginationMetadata
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EnrollmentResponseDTO>>> GetEnrollmentById(Guid id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (enrollment == null)
            {
                return NotFound(ApiResponseHelper.Error<EnrollmentResponseDTO>("Enrollment not found", 404));
            }

            // cek hak akses
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserRole == "Student")
            {
                if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out int userId))
                {
                    return BadRequest(ApiResponseHelper.Error<EnrollmentResponseDTO>("Invalid user ID", 400));
                }
                
                var studentId = await _enrollmentService.GetStudentIdFromUserIdAsync(userId);
                if (studentId == null || enrollment.IdStudent != studentId)
                    return Forbid("Students can only view their own enrollment");
            }
            else if (currentUserRole == "Teacher")
            {
                if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out int userId))
                {
                    return BadRequest(ApiResponseHelper.Error<EnrollmentResponseDTO>("Invalid user ID", 400));
                }
                
                var teacherId = await _enrollmentService.GetTeacherIdFromUserIdAsync(userId);
                if (teacherId == null)
                    return BadRequest(ApiResponseHelper.Error<EnrollmentResponseDTO>("Teacher not found", 400));
                    
                var hasAccess = await _enrollmentService.IsTeacherAuthorizedForEnrollmentAsync(teacherId.Value, id);
                if (!hasAccess)
                    return Forbid("Teachers can only view enrollments for their assigned classes");
            }

            return Ok(ApiResponseHelper.Success(enrollment, "Enrollment retrieved successfully"));
        }

        [HttpGet("my-classes")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<ApiResponse<List<EnrollmentResponseDTO>>>> GetMyClassEnrollments()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out int userId))
            {
                return BadRequest(ApiResponseHelper.Error<List<EnrollmentResponseDTO>>("Invalid user ID", 400));
            }
            
            var enrollments = await _enrollmentService.GetEnrollmentsByUserIdAsync(userId, currentUserRole!);
            return Ok(ApiResponseHelper.Success(enrollments, $"{enrollments.Count} enrollments retrieved successfully"));
        }

        [HttpGet("my-enrollment")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApiResponse<List<EnrollmentResponseDTO>>>> GetMyEnrollment()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out int userId))
            {
                return BadRequest(ApiResponseHelper.Error<List<EnrollmentResponseDTO>>("Invalid user ID", 400));
            }
            
            var enrollments = await _enrollmentService.GetEnrollmentsByUserIdAsync(userId, currentUserRole!);
            return Ok(ApiResponseHelper.Success(enrollments, $"{enrollments.Count} enrollments retrieved successfully"));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult<ApiResponse<EnrollmentResponseDTO>>> CreateEnrollment([FromBody] CreateEnrollmentDTO createEnrollmentDto)
        {
            var enrollment = await _enrollmentService.CreateEnrollmentAsync(createEnrollmentDto);
            return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.Id }, 
                ApiResponseHelper.Success(enrollment, "Enrollment created successfully", 201));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult<ApiResponse<EnrollmentResponseDTO>>> UpdateEnrollment(Guid id, [FromBody] CreateEnrollmentDTO updateEnrollmentDto)
        {
            var enrollment = await _enrollmentService.UpdateEnrollmentAsync(id, updateEnrollmentDto);
            if (enrollment == null)
            {
                return NotFound(ApiResponseHelper.Error<EnrollmentResponseDTO>("Enrollment not found", 404));
            }
            
            return Ok(ApiResponseHelper.Success(enrollment, "Enrollment updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteEnrollment(Guid id)
        {
            var result = await _enrollmentService.DeleteEnrollmentAsync(id);
            if (!result)
            {
                return NotFound(ApiResponseHelper.Error<object>("Enrollment not found", 404));
            }
            
            return Ok(ApiResponseHelper.Success<object>(new { }, "Enrollment deleted successfully"));
        }
    }
}
