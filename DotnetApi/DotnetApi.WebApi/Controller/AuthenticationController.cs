using DotnetApi.Application.Authentication.Command;
using DotnetApi.Application.User.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet()]
    public async Task<IActionResult> GetAllUsers() => Ok(await _mediator.Send(new GetUserQuery()));
}