using DotnetApi.Application.Businesses.Commands;
using FluentValidation;

namespace DotnetApi.Application.Businesses.Validators;

public class AddBusinessValidator : AbstractValidator<AddBusinessCommand>
{
    public AddBusinessValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Industry).NotEmpty();
    }
}