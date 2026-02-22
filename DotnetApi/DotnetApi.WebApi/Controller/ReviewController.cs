using DotnetApi.Application.Reviews.Commands;
using DotnetApi.Application.Reviews.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.WebApi.Controller;


[ApiController]
[Route("api/v1/review")]
public class ReviewController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] AddReviewCommand request)
    {
        var id = await mediator.Send(request);
        return Created($"/api/v1/review/{id}", new{id});
    }
    
    //TODO: Implement the review get
    
    [HttpGet]
    public async Task<IActionResult> GetReview(Guid businessId, string? rawText, string? submitedOn, CancellationToken cancellationToken)
    {
        var review = await mediator.Send(new GetReviewQuery(businessId, rawText, submitedOn), cancellationToken);
        return Ok(review);
    }
}