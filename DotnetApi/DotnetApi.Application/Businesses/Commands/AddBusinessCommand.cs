using MediatR;

namespace DotnetApi.Application.Businesses.Commands;

public record AddBusinessCommand(
    string Name,
    string Industry
): IRequest<Guid>;