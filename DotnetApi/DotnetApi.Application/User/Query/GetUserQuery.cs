using MediatR;

namespace DotnetApi.Application.User.Query;

public record GetUserQuery() : IRequest<List<Domains.Entities.User?>>;