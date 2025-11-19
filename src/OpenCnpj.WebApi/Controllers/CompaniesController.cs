using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
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
            return BadRequest("Não foi possível retornar as empresas com os filtros fornecidos", result.Error);

        if (!result.Value.Any())
            return NotFound("Não foi possível localizar empresas com base nos filtros fornecidos.");

        return Ok(result.Value);
    }

    [HttpGet("{cnpj}")]
    public async Task<IActionResult> GetByCnpj(string cnpj, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
