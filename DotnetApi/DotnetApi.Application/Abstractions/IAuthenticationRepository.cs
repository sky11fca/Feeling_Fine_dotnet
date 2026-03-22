using DotnetApi.Domains.Entities;

namespace DotnetApi.Application.Abstractions;

public interface IAuthenticationRepository
{
    Task<User?> Register(User user, CancellationToken cancellationToken = default);
    Task<User?> Login(string email, string password, CancellationToken cancellationToken = default);
}