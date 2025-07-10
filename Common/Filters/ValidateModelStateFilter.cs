using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolManagementSystem.Common.Models;
using SchoolManagementSystem.Common.Helpers;

namespace SchoolManagementSystem.Common.Filters
{
    public class ValidateModelStateFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var errorMessages = errors.SelectMany(e => e.Value).ToList();
                var errorMessage = string.Join(", ", errorMessages);

                var response = ApiResponseHelper.Error<object>(errorMessage, 400);
                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after execution
        }
    }
}
