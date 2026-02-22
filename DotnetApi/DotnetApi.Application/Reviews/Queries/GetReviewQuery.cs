using MediatR;

namespace DotnetApi.Application.Reviews.Queries;

public record GetReviewQuery(
    Guid BusinessId,
    string? RawText,
    string? SubmitedOn
    ): IRequest<List<ReviewDto>>;