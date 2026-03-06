using DotnetApi.Application.Abstractions;
using MediatR;

namespace DotnetApi.Application.Clients.Query;

public class GetAllClientsQueryHandler(IClientRepository repository): IRequestHandler<GetAllClientsQuery, List<ClientDto>>
{
    public Task<List<ClientDto>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
    {
        var clients =  repository.Query().Select(x => new ClientDto(x.Id, x.Username, x.Email, x.PhoneNumber)).ToList();
        return Task.FromResult(clients);
    }
}