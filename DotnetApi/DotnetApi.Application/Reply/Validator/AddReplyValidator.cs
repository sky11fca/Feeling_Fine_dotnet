using DotnetApi.Application.Reply.Command;
using FluentValidation;

namespace DotnetApi.Application.Reply.Validator;

public class AddReplyValidator : AbstractValidator<AddReplyCommand>
{
    public AddReplyValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty().NotNull();
        RuleFor(x => x.ToClientId).NotEmpty().NotNull();
        RuleFor(x => x.RawText).NotEmpty();
    }
}