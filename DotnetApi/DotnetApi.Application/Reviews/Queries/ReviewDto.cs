namespace DotnetApi.Application.Reviews.Queries;

public record ReviewDto(
    Guid Id,
    string RawText,
    string SubmitedOn
    );