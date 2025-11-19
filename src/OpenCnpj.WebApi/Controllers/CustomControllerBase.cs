using Microsoft.AspNetCore.Mvc;
using OpenCnpj.Application.Application.Models.Dtos;
using System.Net;

namespace OpenCnpj.WebApi.Controllers;

public class CustomControllerBase : ControllerBase
{
    [NonAction]
    public virtual IActionResult BadRequest(string title, string description)
    {
        int statusCode = (int)HttpStatusCode.BadRequest;

        return new ObjectResult(new ApplicationErrorDto("Bad Request", title, statusCode, Guid.CreateVersion7(), description))
        {
            StatusCode = statusCode
        };
    }

    [NonAction]
    public virtual IActionResult NotFound(string title)
    {
        int statusCode = (int)HttpStatusCode.NotFound;

        return new ObjectResult(new ApplicationErrorDto("Not Found", title, statusCode, Guid.CreateVersion7(), string.Empty))
        {
            StatusCode = statusCode
        };
    }
}
