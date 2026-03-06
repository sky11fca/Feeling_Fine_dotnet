using DotnetApi.Application.Abstractions;
using MediatR;

namespace DotnetApi.Application.Clients.Query;

public class GetClientByIdQueryHandler(IClientRepository repository) : IRequestHandler<GetClientByIdQuery, ClientDto>
{
    public async Task<ClientDto> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await repository.FindAsync(request.Id, cancellationToken);

        if (client == null)
        {
            throw new KeyNotFoundException($"Client with Id {request.Id} was not found.");
        }

        return new ClientDto(client.Id, client.Username, client.Email, client.PhoneNumber);
    }
}
