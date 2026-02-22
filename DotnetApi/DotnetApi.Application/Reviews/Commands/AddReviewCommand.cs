using MediatR;

namespace DotnetApi.Application.Reviews.Commands;

public record AddReviewCommand(
    Guid BusinessId,
    string RawText,
    string SubmitedOn
    ): IRequest<Guid>;