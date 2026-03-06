using MediatR;

namespace DotnetApi.Application.Clients.Query;

public record GetAllClientsQuery() : IRequest<List<ClientDto>>;