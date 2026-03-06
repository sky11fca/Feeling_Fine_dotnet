using MediatR;

namespace DotnetApi.Application.Clients.Query;

public record GetClientByIdQuery(Guid Id): IRequest<ClientDto>;