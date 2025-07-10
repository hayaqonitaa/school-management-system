using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolManagementSystem.Common.Models;
using SchoolManagementSystem.Common.Helpers;

namespace SchoolManagementSystem.Common.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "An error occurred while processing your request.";

            if (exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = exception.Message;
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = StatusCodes.Status403Forbidden;
                message = "You don't have permission to access this resource.";
            }
            else if (exception is KeyNotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                message = "The requested resource was not found.";
            }

            // Log the exception
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            // Create API response
            var response = ApiResponseHelper.Error<object>(message, statusCode);
            
            // Set the result
            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };

            // Mark exception as handled
            context.ExceptionHandled = true;
        }
    }
}
