using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Classes.Dtos;
using SchoolManagementSystem.Modules.Classes.Services;
using System.Security.Claims;
using SchoolManagementSystem.Common.Models;
using SchoolManagementSystem.Common.Helpers;

namespace SchoolManagementSystem.Modules.Classes.Controllers
{
    [Route("api/classes")]
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
        public async Task<ActionResult<ApiResponse<List<ClassResponseDTO>>>> GetAllClasses()
        {
            try
            {
                var classes = await _classService.GetAllClassesAsync();
                var response = ApiResponseHelper.Success(classes, $"{classes.Count} classes retrieved successfully.");
                return Ok(response);
            }
            catch (Exception)
            {
                var response = ApiResponseHelper.Error<List<ClassResponseDTO>>("An error occurred while retrieving classes.", 500);
                return StatusCode(500, response);
            }
        }

        [HttpGet("paginated")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<ApiResponse<List<ClassResponseDTO>>>> GetAllClassesPaginated(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var (classes, totalCount) = await _classService.GetAllClassesPaginatedAsync(page, pageSize);
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

                var response = new ApiResponse<List<ClassResponseDTO>>
                {
                    Success = true,
                    Message = $"{classes.Count} classes retrieved successfully.",
                    Data = classes,
                    StatusCode = 200,
                    Pagination = paginationMetadata
                };
                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseHelper.Error<List<ClassResponseDTO>>("An error occurred while retrieving the classes.", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<ApiResponse<ClassResponseDTO>>> GetClassById(Guid id)
        {
            try
            {
                var classEntity = await _classService.GetClassByIdAsync(id);
                if (classEntity == null)
                    return NotFound(ApiResponseHelper.Error<ClassResponseDTO>("Class not found", StatusCodes.Status404NotFound));

                return Ok(ApiResponseHelper.Success(classEntity, "Class retrieved successfully."));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseHelper.Error<ClassResponseDTO>("An error occurred while retrieving the class.", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ClassResponseDTO>>> CreateClass([FromBody] CreateClassDTO createClassDto)
        {
            try
            {
                var classEntity = await _classService.CreateClassAsync(createClassDto);
                return CreatedAtAction(nameof(GetClassById), new { id = classEntity.Id }, 
                    ApiResponseHelper.Success(classEntity, "Class created successfully.", StatusCodes.Status201Created));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseHelper.Error<ClassResponseDTO>(ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseHelper.Error<ClassResponseDTO>("An error occurred while creating the class.", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ClassResponseDTO>>> UpdateClass(Guid id, [FromBody] CreateClassDTO updateClassDto)
        {
            try
            {
                var classEntity = await _classService.UpdateClassAsync(id, updateClassDto);
                if (classEntity == null)
                    return NotFound(ApiResponseHelper.Error<ClassResponseDTO>("Class not found", StatusCodes.Status404NotFound));

                return Ok(ApiResponseHelper.Success(classEntity, "Class updated successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseHelper.Error<ClassResponseDTO>(ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseHelper.Error<ClassResponseDTO>("An error occurred while updating the class.", StatusCodes.Status500InternalServerError));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteClass(Guid id)
        {
            try
            {
                var result = await _classService.DeleteClassAsync(id);
                if (!result)
                    return NotFound(ApiResponseHelper.Error<object>("Class not found", StatusCodes.Status404NotFound));

                return Ok(ApiResponseHelper.Success<object>(new { }, "Class deleted successfully."));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseHelper.Error<object>("An error occurred while deleting the class.", StatusCodes.Status500InternalServerError));
            }
        }
    }
}
