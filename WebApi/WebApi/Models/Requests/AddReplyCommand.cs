namespace WebApi.Models.Requests;

public record AddReplyCommand(
    Guid ReviewId,
    Guid ToClientId,
    string RawText);