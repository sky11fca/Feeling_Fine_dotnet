using DotnetApi.Application.Abstractions;
using DotnetApi.Domains.Entities;
using MediatR;

namespace DotnetApi.Application.Businesses.Commands;

public sealed class AddBusinessCommandHandler(IBusinessRepository repository) : IRequestHandler<AddBusinessCommand, Guid> //I can guarantee this class is not inheritable
{
    public async Task<Guid> Handle(AddBusinessCommand request, CancellationToken cancellationToken)
    {
        var business = Business.Create(request.Name, request.Industry);
        await repository.AddAsync(business, cancellationToken);
        return business.Id;
    }
}