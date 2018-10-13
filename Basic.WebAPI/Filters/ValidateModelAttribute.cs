using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Basic.WebAPI.Controllers;
using Basic.WebAPI.Helpers;
using Basic.WebAPI.ViewModels;

namespace Basic.WebAPI.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errorList = WebHelper.GetCustomModelErrores(context.ModelState);
                context.Result = new OkObjectResult(errorList);
                return;
            }
        }
    }
}
