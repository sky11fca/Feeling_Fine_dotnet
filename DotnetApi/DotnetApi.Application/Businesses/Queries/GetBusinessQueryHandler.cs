using DotnetApi.Application.Abstractions;
using MediatR;

namespace DotnetApi.Application.Businesses.Queries;

public class GetBusinessQueryHandler(IBusinessRepository repository) : IRequestHandler<GetBusinessQuery, List<BusinessDto>>
{
    public Task<List<BusinessDto>> Handle(GetBusinessQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Query();

        if (!string.IsNullOrWhiteSpace(request.Name) || !string.IsNullOrWhiteSpace(request.Industry))
        {
            query = query.Where(x =>
                (!string.IsNullOrWhiteSpace(request.Name) && x.Name.Contains(request.Name)) ||
                (!string.IsNullOrWhiteSpace(request.Industry) && x.Industry.Contains(request.Industry)));
        }

        var businesses = query.Select(x => new BusinessDto(x.Id, x.Name, x.Industry)).ToList();
        return Task.FromResult(businesses);
    }
}