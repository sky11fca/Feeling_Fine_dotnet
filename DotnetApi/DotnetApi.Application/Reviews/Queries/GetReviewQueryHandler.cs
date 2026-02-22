using DotnetApi.Application.Abstractions;
using MediatR;

namespace DotnetApi.Application.Reviews.Queries;

public class GetReviewQueryHandler(IReviewRepository repository): IRequestHandler<GetReviewQuery, List<ReviewDto>>
{
    public Task<List<ReviewDto>> Handle(GetReviewQuery request, CancellationToken cancellationToken)
    {
        if (request.BusinessId == default)
        {
            throw new ArgumentException("Business ID is required");
        }

        var query = repository.Query().Where(x => x.BusinessId == request.BusinessId);

        if (!string.IsNullOrWhiteSpace(request.RawText) || !string.IsNullOrWhiteSpace(request.SubmitedOn))
        {
            query = query.Where(x =>
                (!string.IsNullOrWhiteSpace(request.RawText) && x.RawText.Contains(request.RawText)) ||
                (!string.IsNullOrWhiteSpace(request.SubmitedOn) && x.SubmitedOn.Contains(request.SubmitedOn)));
        }

        var reviews = query.Select(x => new ReviewDto(x.Id, x.RawText, x.SubmitedOn)).ToList();

        return Task.FromResult(reviews);
    }
}