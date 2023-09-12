using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MovieBooking.API.Filters
{
    public class NullCheckFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var parameter = context.ActionArguments.FirstOrDefault();
            if(parameter.Value is null)
            {
                context.Result = new BadRequestObjectResult("Null parameter passed");
            }
            
            if(!context.ModelState.IsValid)
            {
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }
    }
}
