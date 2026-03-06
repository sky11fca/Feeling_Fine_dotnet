using MediatR;

namespace DotnetApi.Application.Clients.Commands;

public record AddClientCommand(
    string Username,
    string Email,
    string PhoneNumber
    ) : IRequest<Guid>;