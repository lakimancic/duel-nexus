using Microsoft.AspNetCore.Mvc;

namespace Backend.Utils.WebApi;

public class ExceptionHandler
{
    public static IActionResult HandleException(Exception ex)
    {
        return ex switch
        {
            ObjectNotFoundException => new NotFoundObjectResult(new { error = ex.Message }),
            BadRequestException => new BadRequestObjectResult(new { error = ex.Message }),
            ConflictObjectException => new ConflictObjectResult(new { error = ex.Message }),
            _ => new ObjectResult(new { error = "Something went wrong on the server" }) { StatusCode = 500 }
        };
    }
}
