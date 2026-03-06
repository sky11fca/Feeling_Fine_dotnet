using DotnetApi.Application.Businesses.Commands;
using DotnetApi.Application.Businesses.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.WebApi.Controller;

[ApiController]
[Route("api/v1/business")]
public class BusinessController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddBusiness([FromBody] AddBusinessCommand request, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(request, cancellationToken);
        return Created($"/api/v1/business/{id}", new{id});
    }

    [HttpGet]
    public async Task<IActionResult> GetBusiness(string? name, string? industry, CancellationToken cancellationToken)
    {
        var business = await mediator.Send(new GetBusinessQuery(name, industry), cancellationToken);
        return Ok(business);
    }
}