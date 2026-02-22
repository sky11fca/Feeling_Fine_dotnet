using DotnetApi.Application.Abstractions;
using DotnetApi.Domains.Entities;
using MediatR;

namespace DotnetApi.Application.Reviews.Commands;

public class AddReviewCommandHandler(IReviewRepository repository) : IRequestHandler<AddReviewCommand, Guid>
{
    public async Task<Guid> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        var review = Review.Create(request.BusinessId, request.RawText, request.SubmitedOn);
        await repository.AddAsync(review, cancellationToken);
        return review.Id;
    }
}