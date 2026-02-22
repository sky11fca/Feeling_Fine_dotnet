namespace DotnetApi.Application.Businesses.Queries;

public record BusinessDto(
    Guid Id,
    string Name,
    string Industry
    );