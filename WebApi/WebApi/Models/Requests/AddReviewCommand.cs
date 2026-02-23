namespace WebApi.Models.Requests;

public record AddReviewCommand(
    Guid BusinessId,
    decimal Review,
    string RawText,
    string SubmitedOn
    );