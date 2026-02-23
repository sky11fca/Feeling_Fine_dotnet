using DotnetApi.Application.Reviews.Commands;
using FluentValidation;

namespace DotnetApi.Application.Reviews.Validators;

public class AddReviewValidator : AbstractValidator<AddReviewCommand>
{
    public AddReviewValidator()
    {
        RuleFor(x => x.BusinessId).NotEmpty();
        RuleFor(x => x.Review).LessThanOrEqualTo(1.0m).GreaterThanOrEqualTo(5.0m);
        RuleFor(x => x.RawText).NotEmpty();
        RuleFor(x => x.SubmitedOn).NotEmpty();
    }
}