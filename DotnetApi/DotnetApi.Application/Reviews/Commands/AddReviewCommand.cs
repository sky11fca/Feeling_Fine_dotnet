using MediatR;

namespace DotnetApi.Application.Reviews.Commands;

public record AddReviewCommand(
    Guid BusinessId,
    decimal Review,
    string RawText,
    string SubmitedOn
    ): IRequest<Guid>;