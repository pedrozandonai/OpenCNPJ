using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OpenCnpj.Application.Application.Commands;
using OpenCnpj.Core;

namespace OpenCnpj.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class ApplicationController(IMediator mediator) : CustomControllerBase
{
    [HttpPost("execute")]
    public async Task<IActionResult> PostManualApplicationScrapper(PostManualApplicationScrapperCommand command,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return BadRequest("An error ocurred while manually trying to update the data from the application.", result.Error);

        return Ok();
    }
}
