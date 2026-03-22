using DotnetApi.Application.Authentication.Command;
using FluentValidation;

namespace DotnetApi.Application.Authentication.Validation;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.BusinessId).NotEmpty().NotNull();
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}