using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Students.Dtos;
using SchoolManagementSystem.Modules.Students.Services;
using SchoolManagementSystem.Common.Models;
using SchoolManagementSystem.Common.Helpers;

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
        public async Task<ActionResult<ApiResponse<StudentResponseDTO>>> CreateStudent([FromBody] CreateStudentDTO createStudentDto)
        {
            var student = await _studentService.CreateStudentAsync(createStudentDto);
            var response = ApiResponseHelper.Success(student, "Student created successfully", 201);
            return StatusCode(201, response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<StudentResponseDTO>>> GetStudentById(Guid id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                var notFoundResponse = ApiResponseHelper.Error<StudentResponseDTO>("Student not found.", 404);
                return NotFound(notFoundResponse);
            }
            var response = ApiResponseHelper.Success(student, "Student retrieved successfully");
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<StudentResponseDTO>>>> GetAllStudents()
        {
            var students = await _studentService.GetAllStudentsAsync();
            var response = ApiResponseHelper.Success(students, $"{students.Count} students retrieved successfully.");
            return Ok(response);
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<ApiResponse<List<StudentResponseDTO>>>> GetAllStudentsPaginated(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var (students, totalCount) = await _studentService.GetAllStudentsPaginatedAsync(page, pageSize);
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

            var response = new ApiResponse<List<StudentResponseDTO>>
            {
                Success = true,
                Message = $"{students.Count} students retrieved successfully.",
                Data = students,
                StatusCode = 200,
                Pagination = paginationMetadata
            };
            return Ok(response);
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<StudentResponseDTO>>> UpdateStudent(Guid id, [FromBody] CreateStudentDTO updateStudentDto)
        {
            var student = await _studentService.UpdateStudentAsync(id, updateStudentDto);
            if (student == null)
            {
                var notFoundResponse = ApiResponseHelper.Error<StudentResponseDTO>("Student not found.", 404);
                return NotFound(notFoundResponse);
            }
            var response = ApiResponseHelper.Success(student, "Student updated successfully.");
            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStudent(Guid id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            if (!result)
            {
                var notFoundResponse = ApiResponseHelper.Error<object>("Student not found.", 404);
                return NotFound(notFoundResponse);
            }
            var response = ApiResponseHelper.Success<object>(new { }, "Student deleted successfully.");
            return Ok(response);
        }
    }
}
