using DotnetApi.Domains.Entities;

namespace DotnetApi.Application.Abstractions;

public interface IAuthenticationRepository
{
    Task<Domains.Entities.User?> Register(Domains.Entities.User user, CancellationToken cancellationToken = default);
    Task<Domains.Entities.User?> Login(string email, string password, CancellationToken cancellationToken = default);
}