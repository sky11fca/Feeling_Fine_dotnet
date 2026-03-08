namespace DotnetApi.Application.Reply.Query;

public record ReplyDto(
    Guid ReviewId,
    Guid ToClientId, 
    string RawText
    );