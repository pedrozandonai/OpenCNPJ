using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OpenCnpj.Core;

namespace OpenCnpj.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class CompaniesController(IMediator mediator) : CustomControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByFilters(string cnpj, CancellationToken cancellationToken)
    {
        return Ok();
    }

    [HttpGet("{cnpj}")]
    public async Task<IActionResult> GetByCnpj(string cnpj, CancellationToken cancellationToken)
    {
        return Ok();
    }
}
