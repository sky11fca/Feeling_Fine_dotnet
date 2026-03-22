using DotnetApi.Domains.Entities;

namespace DotnetApi.Application.Abstractions;

public interface IUserRepository
{
    Task<Domains.Entities.User?> AddUserAsync(Domains.Entities.User user, CancellationToken cancellationToken = default);
    Task<Domains.Entities.User?> GetUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Domains.Entities.User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}