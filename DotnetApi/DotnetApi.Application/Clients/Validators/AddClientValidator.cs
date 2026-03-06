using DotnetApi.Application.Clients.Commands;
using FluentValidation;

namespace DotnetApi.Application.Clients.Validators;

public class AddClientValidator : AbstractValidator<AddClientCommand>
{
    public AddClientValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty()
            .Matches(@"^\+[1-9]\d{1,14}$").WithMessage("Phone number must be in E.164 format (e.g., +12223334444).");
    }
}