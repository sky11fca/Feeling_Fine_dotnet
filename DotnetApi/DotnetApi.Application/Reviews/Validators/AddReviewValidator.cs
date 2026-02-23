using DotnetApi.Application.Reviews.Commands;
using FluentValidation;

namespace DotnetApi.Application.Reviews.Validators;

public class AddReviewValidator : AbstractValidator<AddReviewCommand>
{
    public AddReviewValidator()
    {
        RuleFor(x => x.BusinessId).NotEmpty();
        RuleFor(x => x.Review).GreaterThanOrEqualTo(1.0m).LessThanOrEqualTo(5.0m);
        RuleFor(x => x.RawText).NotEmpty();
        RuleFor(x => x.SubmitedOn).NotEmpty();
    }
}