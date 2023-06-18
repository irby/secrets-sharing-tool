using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SecretsSharingTool.Web.Filters;

public class ExceptionFilter: ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception.GetType() == typeof(ValidationException))
        {
            context.Result = new BadRequestObjectResult(new BadRequestResult
            {
                Message = context.Exception.Message
            });
        }
        else
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

public class BadRequestResult
{
    public string Message { get; set; }
}