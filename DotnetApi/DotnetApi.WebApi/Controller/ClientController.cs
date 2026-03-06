using DotnetApi.Application.Clients.Commands;
using DotnetApi.Application.Clients.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.WebApi.Controller;

[ApiController]
[Route("api/v1/client")]
public class ClientController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddClient([FromBody] AddClientCommand request, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(request, cancellationToken);
        return Created($"/api/v1/client/{id}", new{id});
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetClient(Guid id, CancellationToken cancellationToken)
    {
        var client = await mediator.Send(new GetClientByIdQuery(id), cancellationToken);
        return Ok(client);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClient(CancellationToken cancellationToken)
    {
        var clients = await mediator.Send(new GetAllClientsQuery(), cancellationToken);
        return Ok(clients);
    }
}