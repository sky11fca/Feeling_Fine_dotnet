using DotnetApi.Application.Authentication.Command;
using FluentValidation;

namespace DotnetApi.Application.Authentication.Validation;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}