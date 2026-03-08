using DotnetApi.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace DotnetApi.Application.Reply.Command;

public class AddReplyCommandHandler(IReplyRepository repository, IValidator<AddReplyCommand> validator) : IRequestHandler<AddReplyCommand, Domains.Entities.Reply>
{
    public async Task<Domains.Entities.Reply> Handle(AddReplyCommand request, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        var reply = DotnetApi.Domains.Entities.Reply.Create(request.ReviewId, request.ToClientId, request.RawText);
        await repository.AddAsync(reply, cancellationToken);
        return reply;
    }
}