namespace WebApi.Models.Requests;

public record AddReviewCommand(
    Guid BusinessId,
    Guid ClientId,
    decimal Review,
    string RawText,
    string SubmitedOn
    );