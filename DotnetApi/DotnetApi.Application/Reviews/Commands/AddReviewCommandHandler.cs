using DotnetApi.Application.Abstractions;
using DotnetApi.Domains.Entities;
using DotnetApi.Domains.Enums;
using FluentValidation;
using MediatR;

namespace DotnetApi.Application.Reviews.Commands;

public class AddReviewCommandHandler(IReviewRepository repository, IValidator<AddReviewCommand> validator) : IRequestHandler<AddReviewCommand, Guid>
{
    public async Task<Guid> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
        
        var finalReviewType = request.Review switch
        {
            >= 5.0m => RatingType.OverwhelminglyPositive,
            >= 4.0m => RatingType.MostlyPositive,
            >= 3.0m => RatingType.Mixed,
            >= 2.0m => RatingType.MostlyNegative,
            _ => RatingType.OverwhelminglyNegative
        };
        
        var review = Review.Create(request.BusinessId, request.ClientId, request.Review, finalReviewType, request.RawText, request.SubmitedOn);
        await repository.AddAsync(review, cancellationToken);
        return review.Id;
    }
}