using DotnetApi.Application.Reviews.Commands;
using FluentValidation;

namespace DotnetApi.Application.Reviews.Validators;

public class AddReviewValidator : AbstractValidator<AddReviewCommand>
{
    public AddReviewValidator()
    {
        RuleFor(x => x.BusinessId).NotEmpty();
        RuleFor(x => x.RawText).NotEmpty();
        RuleFor(x => x.SubmitedOn).NotEmpty();
    }
}