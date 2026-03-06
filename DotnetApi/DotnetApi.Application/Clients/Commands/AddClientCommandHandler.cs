using DotnetApi.Application.Abstractions;
using DotnetApi.Domains.Entities;
using FluentValidation;
using MediatR;

namespace DotnetApi.Application.Clients.Commands;

public class AddClientCommandHandler(IClientRepository repository, IValidator<AddClientCommand> validator): IRequestHandler<AddClientCommand, Guid>
{
    public async Task<Guid> Handle(AddClientCommand request, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors.ToString());
        }
        
        var client = Client.Create(request.Username, request.Email, request.PhoneNumber);
        await repository.AddAsync(client, cancellationToken);
        return client.Id;
    }
}