using DotnetApi.Application.Reply.Command;
using DotnetApi.Application.Reply.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.WebApi.Controller;

[ApiController]
[Route("api/v1/reply")]
public class ReplyController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddReply([FromBody] AddReplyCommand req)
    {
        var resp = await mediator.Send(req);
        return Created($"/api/v1/reply/{resp.Id}", new{resp.Id});
    }

    [HttpGet]
    public async Task<IActionResult> GetReplies(Guid reviewId, CancellationToken cancellationToken)
    {
        var resp = await mediator.Send(new GetRepliesQuery(reviewId), cancellationToken);
        return Ok(resp);
    }
}