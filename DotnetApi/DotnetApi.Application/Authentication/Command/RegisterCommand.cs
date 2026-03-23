using DotnetApi.Domains.Entities;
using MediatR;

namespace DotnetApi.Application.Authentication.Command;

public record RegisterCommand(
    string Username,
    Guid BusinessId,
    string Email,
    string Password,
    string UserRole
    ): IRequest<Domains.Entities.User?>;