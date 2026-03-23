using DotnetApi.Application.Abstractions;
using MediatR;

namespace DotnetApi.Application.User.Query;

public class GetUserQueryHandler(IUserRepository userRepository): IRequestHandler<GetUserQuery, List<Domains.Entities.User?>>
{
    public async Task<List<Domains.Entities.User?>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetUsersAsync(cancellationToken);
        return await Task.FromResult(users);
    }
}