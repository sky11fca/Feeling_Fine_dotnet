using DotnetApi.Application.Abstractions;
using DotnetApi.Domains.Entities;

namespace DotnetApi.Infrastructure.Repository;

public class AuthenticationRepository : IAuthenticationRepository
{
    public Task<User?> Register(User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> Login(string email, string password, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}