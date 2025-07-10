using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Teachers.Dtos;
using SchoolManagementSystem.Modules.Teachers.Services;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Common.Models;
using SchoolManagementSystem.Common.Helpers;

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
        public async Task<ActionResult<ApiResponse<TeacherResponseDTO>>> CreateTeacher([FromBody] CreateTeacherDTO createTeacherDto)
        {
            try
            {
                var teacher = await _teacherService.CreateTeacherAsync(createTeacherDto);
                var response = ApiResponseHelper.Success(teacher, "Teacher created successfully.", 201);
                return StatusCode(201, response);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponseHelper.Error<TeacherResponseDTO>(ex.Message, 400);
                return BadRequest(response);
            }
            catch (Exception)
            {
                var response = ApiResponseHelper.Error<TeacherResponseDTO>("An error occurred while creating the teacher.", 500);
                return StatusCode(500, response);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<TeacherResponseDTO>>> GetTeacherById(Guid id)
        {
            try
            {
                var teacher = await _teacherService.GetTeacherByIdAsync(id);
                if (teacher == null)
                {
                    var notFoundResponse = ApiResponseHelper.Error<TeacherResponseDTO>("Teacher not found.", 404);
                    return NotFound(notFoundResponse);
                }
                var response = ApiResponseHelper.Success(teacher, "Teacher retrieved successfully.");
                return Ok(response);
            }
            catch (Exception)
            {
                var response = ApiResponseHelper.Error<TeacherResponseDTO>("An error occurred while retrieving the teacher.", 500);
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<TeacherResponseDTO>>>> GetAllTeachers()
        {
            try
            {
                var teachers = await _teacherService.GetAllTeachersAsync();
                if (teachers == null || teachers.Count == 0)
                {
                    var response = ApiResponseHelper.Success(new List<TeacherResponseDTO>(), "No teachers found.");
                    return Ok(response);
                }
                var successResponse = ApiResponseHelper.Success(teachers, $"{teachers.Count} teachers retrieved successfully.");
                return Ok(successResponse);
            }
            catch (Exception)
            {
                var response = ApiResponseHelper.Error<List<TeacherResponseDTO>>("An error occurred while retrieving the teachers.", 500);
                return StatusCode(500, response);
            }
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<ApiResponse<List<TeacherResponseDTO>>>> GetAllTeachersPaginated(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var (teachers, totalCount) = await _teacherService.GetAllTeachersPaginatedAsync(page, pageSize);
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

                var response = new ApiResponse<List<TeacherResponseDTO>>
                {
                    Success = true,
                    Message = $"{teachers.Count} teachers retrieved successfully.",
                    Data = teachers,
                    StatusCode = 200,
                    Pagination = paginationMetadata
                };
                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseHelper.Error<List<TeacherResponseDTO>>("An error occurred while retrieving the teachers.", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<TeacherResponseDTO>>> UpdateTeacher(Guid id, [FromBody] CreateTeacherDTO updateTeacherDto)
        {
            try
            {
                var updatedTeacher = await _teacherService.UpdateTeacherAsync(id, updateTeacherDto);
                if (updatedTeacher == null)
                {
                    return NotFound(ApiResponseHelper.Error<TeacherResponseDTO>("Teacher not found.", StatusCodes.Status404NotFound));
                }
                return Ok(ApiResponseHelper.Success(updatedTeacher, "Teacher updated successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseHelper.Error<TeacherResponseDTO>(ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseHelper.Error<TeacherResponseDTO>("An error occurred while updating the teacher.", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTeacher(Guid id)
        {
            try
            {
                var result = await _teacherService.DeleteTeacherAsync(id);
                if (!result)
                {
                    return NotFound(ApiResponseHelper.Error<object>("Teacher not found.", StatusCodes.Status404NotFound));
                }
                return Ok(ApiResponseHelper.Success<object>(new { }, "Teacher deleted successfully."));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseHelper.Error<object>("An error occurred while deleting the teacher.", StatusCodes.Status500InternalServerError));
            }
        }

    }
}