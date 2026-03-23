using DotnetApi.Domains.Entities;

namespace DotnetApi.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateToken(Domains.Entities.User user);
}