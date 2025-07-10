using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Services;
using SchoolManagementSystem.Modules.Classes.Entities;
using SchoolManagementSystem.Common.Models;
using SchoolManagementSystem.Common.Helpers;

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
        public async Task<ActionResult<ApiResponse<List<ClassTeacherResponseDTO>>>> GetAllClassTeachers()
        {
            try
            {
                var classTeachers = await _classTeacherService.GetAllClassTeachersAsync();
                var response = ApiResponseHelper.Success(classTeachers, $"{classTeachers.Count} class teachers retrieved successfully.");
                return Ok(response);
            }
            catch (Exception)
            {
                var response = ApiResponseHelper.Error<List<ClassTeacherResponseDTO>>("An error occurred while retrieving class teachers.", 500);
                return StatusCode(500, response);
            }
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<ApiResponse<List<ClassTeacherResponseDTO>>>> GetAllClassTeachersPaginated(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var (classTeachers, totalCount) = await _classTeacherService.GetAllClassTeachersPaginatedAsync(page, pageSize);
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

                var response = new ApiResponse<List<ClassTeacherResponseDTO>>
                {
                    Success = true,
                    Message = $"{classTeachers.Count} class teachers retrieved successfully.",
                    Data = classTeachers,
                    StatusCode = 200,
                    Pagination = paginationMetadata
                };
                return Ok(response);
            }
            catch (Exception)
            {
                var response = ApiResponseHelper.Error<List<ClassTeacherResponseDTO>>("An error occurred while retrieving the class teachers.", 500);
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ClassTeacherResponseDTO>>> AssignTeacherToClass([FromBody] AssignTeacherRequestDTO assignRequest)
        {
            try
            {
                var classTeacher = await _classTeacherService.AssignTeacherToClassAsync(
                    assignRequest.IdTeacher, assignRequest.IdClass, assignRequest.Tahun);
                
                if (classTeacher == null)
                {
                    var errorResponse = ApiResponseHelper.Error<ClassTeacherResponseDTO>("Failed to assign teacher to class", 400);
                    return BadRequest(errorResponse);
                }

                var response = ApiResponseHelper.Success(classTeacher, "Teacher assigned to class successfully.", 201);
                return StatusCode(201, response);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponseHelper.Error<ClassTeacherResponseDTO>(ex.Message, 400);
                return BadRequest(response);
            }
            catch (Exception)
            {
                var response = ApiResponseHelper.Error<ClassTeacherResponseDTO>("An error occurred while assigning teacher to class.", 500);
                return StatusCode(500, response);
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ApiResponse<ClassTeacherResponseDTO>>> UpdateClassTeacher(Guid id, [FromBody] CreateClassTeacherDTO updateClassTeacherDto)
        {
            try
            {
                var classTeacher = await _classTeacherService.UpdateClassTeacherAsync(id, updateClassTeacherDto);
                if (classTeacher == null)
                {
                    var notFoundResponse = ApiResponseHelper.Error<ClassTeacherResponseDTO>("Class teacher assignment not found", 404);
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponseHelper.Success(classTeacher, "Class teacher assignment updated successfully.");
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponseHelper.Error<ClassTeacherResponseDTO>(ex.Message, 400);
                return BadRequest(response);
            }
            catch (Exception)
            {
                var response = ApiResponseHelper.Error<ClassTeacherResponseDTO>("An error occurred while updating the class teacher assignment.", 500);
                return StatusCode(500, response);
            }
        }

        [HttpPatch("{id}/unassign")]
        public async Task<ActionResult<ApiResponse<object>>> UnassignTeacherFromClass(Guid id)
        {
            try
            {
                var result = await _classTeacherService.UnassignTeacherFromClassAsync(id);
                if (!result)
                    return NotFound(ApiResponseHelper.Error<object>("Class teacher assignment not found", StatusCodes.Status404NotFound));

                return Ok(ApiResponseHelper.Success<object>(new { }, "Teacher unassigned successfully."));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseHelper.Error<object>("An error occurred while unassigning teacher from class.", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPatch("{id}/assign")]
        public async Task<ActionResult<ApiResponse<object>>> AssignTeacherFromClass(Guid id)
        {
            try
            {
                var result = await _classTeacherService.AssignTeacherFromClassAsync(id);
                if (!result)
                    return NotFound(ApiResponseHelper.Error<object>("Class teacher assignment not found", StatusCodes.Status404NotFound));

                return Ok(ApiResponseHelper.Success<object>(new { }, "Teacher assigned successfully."));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseHelper.Error<object>("An error occurred while assigning teacher to class.", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteClassTeacher(Guid id)
        {
            try
            {
                var result = await _classTeacherService.DeleteClassTeacherAsync(id);
                if (!result)
                    return NotFound(ApiResponseHelper.Error<object>("Class teacher assignment not found", StatusCodes.Status404NotFound));

                return Ok(ApiResponseHelper.Success<object>(new { }, "Class teacher assignment deleted successfully."));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseHelper.Error<object>("An error occurred while deleting the class teacher assignment.", StatusCodes.Status500InternalServerError));
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
