using Microsoft.AspNetCore.Mvc;
using OpenCnpj.Application.AppErrors.Models.Dtos;
using System.Net;

namespace OpenCnpj.WebApi.Controllers;

public class CustomControllerBase : ControllerBase
{
    [NonAction]
    public virtual IActionResult BadRequest(string title, IEnumerable<string> details)
    {
        return new ObjectResult(new AppErrorDto("Bad Request", title, (int)HttpStatusCode.BadRequest, Guid.CreateVersion7(), details))
        {
            StatusCode = (int)HttpStatusCode.BadRequest
        };
    }
}
