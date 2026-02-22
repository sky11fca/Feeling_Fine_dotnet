using MediatR;

namespace DotnetApi.Application.Businesses.Queries;

public record GetBusinessQuery(
    string Name,
    string Industry
    ): IRequest<List<BusinessDto>>;