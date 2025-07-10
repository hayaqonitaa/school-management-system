using SchoolManagementSystem.Common.Models;

namespace SchoolManagementSystem.Common.Helpers
{
    public static class ApiResponseHelper
    {
        public static ApiResponse<T> Success<T>(T data, string message = "Success", int statusCode = 200, PaginationMetadata? pagination = null)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Success = true,
                Message = message,
                Data = data,
                Pagination = pagination
            };
        }

        public static ApiResponse<T> Error<T>(string message, int statusCode = 500, T? data = default)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Success = false,
                Message = message,
                Data = data,
                Pagination = null
            };
        }

        public static ApiResponse<object> Error(string message, int statusCode = 500)
        {
            return new ApiResponse<object>
            {
                StatusCode = statusCode,
                Success = false,
                Message = message,
                Data = null,
                Pagination = null
            };
        }

        public static PaginationMetadata CreatePaginationMetadata(int page, int size, int totalCount)
        {
            var totalPages = (int)Math.Ceiling((double)totalCount / size);
            
            return new PaginationMetadata
            {
                Page = page,
                Size = size,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPrevious = page > 1,
                HasNext = page < totalPages
            };
        }
    }
}
