using Microsoft.AspNetCore.Mvc;

namespace Backend.Utils.WebApi;

public class ExceptionHandler
{
    public static IActionResult HandleException(Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException => new NotFoundObjectResult(new { error = ex.Message }),
            ArgumentException => new BadRequestObjectResult(new { error = ex.Message }),
            InvalidOperationException => new ConflictObjectResult(new { error = ex.Message }),
            _ => new ObjectResult(new { error = ex.Message }) { StatusCode = 500 }
        };
    }
}
