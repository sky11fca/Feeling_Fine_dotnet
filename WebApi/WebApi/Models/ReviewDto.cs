namespace WebApi.Models;

public record ReviewDto(
    Guid Id,
    string RawText,
    string SubmitedOn
    );