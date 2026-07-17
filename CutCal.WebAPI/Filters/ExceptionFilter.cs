using CutCal.Model.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CutCal.WebAPI.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ValidationException validationException:
                context.Result = new BadRequestObjectResult(new
                {
                    errors = validationException.Errors.Select(e => e.ErrorMessage)
                });
                context.ExceptionHandled = true;
                break;

            case ClientException clientException:
                context.Result = new BadRequestObjectResult(new { message = clientException.Message });
                context.ExceptionHandled = true;
                break;

            default:
                _logger.LogError(context.Exception, "Unhandled server error on {Path}", context.HttpContext.Request.Path);
                context.Result = new ObjectResult(new { message = "Server side error, please check logs." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                context.ExceptionHandled = true;
                break;
        }
    }
}
