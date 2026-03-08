namespace WebApi.Models;

public record ReplyDto(
    Guid ReviewId,
    Guid ToClientId,
    string RawText
    );