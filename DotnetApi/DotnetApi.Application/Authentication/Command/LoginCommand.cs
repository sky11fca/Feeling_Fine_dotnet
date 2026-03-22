
using MediatR;

namespace DotnetApi.Application.Authentication.Command;

public record LoginCommand(
    string Email,
    string Password
    ) : IRequest<string?>;