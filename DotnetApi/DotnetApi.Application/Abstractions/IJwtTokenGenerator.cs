using DotnetApi.Domains.Entities;

namespace DotnetApi.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}