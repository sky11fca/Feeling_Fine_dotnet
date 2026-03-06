using MediatR;

namespace DotnetApi.Application.Reviews.Commands;

public record AddReviewCommand(
    Guid BusinessId,
    Guid ClientId,
    decimal Review,
    string RawText,
    string SubmitedOn
    ): IRequest<Guid>;