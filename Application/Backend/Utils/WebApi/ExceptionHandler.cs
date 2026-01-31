using Microsoft.AspNetCore.Mvc;

namespace Backend.Utils.WebApi;

public class ExceptionHandler
{
    public static IActionResult HandleException(Exception ex, ControllerBase controller)
    {
        return ex switch
        {
            KeyNotFoundException => controller.NotFound(new { error = ex.Message }),
            ArgumentException => controller.BadRequest(new { error = ex.Message }),
            InvalidOperationException => controller.Conflict(new { error = ex.Message }),
            _ => controller.StatusCode(500, new { error = ex.Message })
        };
    }
}
