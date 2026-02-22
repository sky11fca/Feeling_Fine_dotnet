namespace WebApi.Models.Requests;

public record AddReviewCommand(
    Guid BusinessId,
    string RawText,
    string SubmitedOn
    );