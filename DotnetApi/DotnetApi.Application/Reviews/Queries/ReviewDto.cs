namespace DotnetApi.Application.Reviews.Queries;

public record ReviewDto(
    Guid Id,
    decimal Review,
    string RatingType,
    string RawText,
    string SubmittedOn,
    string SentimentLabel,
    double SentimentAccuracy
    );