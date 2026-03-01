using DotnetApi.Application.Abstractions;
using DotnetApi.Domains.Entities;
using FluentValidation;
using MediatR;

namespace DotnetApi.Application.Businesses.Commands;

public sealed class AddBusinessCommandHandler(IBusinessRepository repository, IValidator<AddBusinessCommand> validator) : IRequestHandler<AddBusinessCommand, Guid> //I can guarantee this class is not inheritable
{
    public async Task<Guid> Handle(AddBusinessCommand request, CancellationToken cancellationToken)
    {
        
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors.ToString());
        }
        
        var business = Business.Create(request.Name, request.Industry);
        await repository.AddAsync(business, cancellationToken);
        return business.Id;
    }
}