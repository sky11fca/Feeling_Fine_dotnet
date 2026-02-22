namespace WebApi.Models.Requests;

public record AddBusinessCommand(
    string Name,
    string Industry
    );