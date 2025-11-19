using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OpenCnpj.Application.Companies.Commands;
using OpenCnpj.Core;

namespace OpenCnpj.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class CompaniesController(IMediator mediator) : CustomControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByFilters([FromQuery] GetByFiltersCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return BadRequest("An error occured while trying to return the companies by the filters.", result.Error);

        if (!result.Value.Any())
            return NotFound("Unable to find any companies with the informed filters.");

        return Ok(result.Value);
    }

    [HttpGet("{cnpj}")]
    public async Task<IActionResult> GetByCnpj(string cnpj, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetByCnpjCommand(cnpj), cancellationToken);
        if (result.IsFailure)
            return BadRequest("An error occurred while trying to return the company by the CNPJ informed.", result.Error);

        if (result.Value == null)
            return NotFound("Unable to find the requested company by the CNPJ informed.");

        return Ok(result.Value);
    }
}
