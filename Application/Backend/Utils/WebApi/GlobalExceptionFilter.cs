using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.Utils.WebApi;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = ExceptionHandler.HandleException(context.Exception);
        context.ExceptionHandled = true;
    }
}
