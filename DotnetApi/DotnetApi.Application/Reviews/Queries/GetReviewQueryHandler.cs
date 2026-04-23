using DotnetApi.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DotnetApi.Application.Reviews.Queries;

public class GetReviewQueryHandler(IReviewRepository repository, IDistributedCache cache) : IRequestHandler<GetReviewQuery, List<ReviewDto>>
{
    public async Task<List<ReviewDto>> Handle(GetReviewQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"reviews_{request.BusinessId}_{request.RawText}_{request.SubmitedOn}";
        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<List<ReviewDto>>(cachedData) ?? new List<ReviewDto>();
        }

        var query = repository.Query();

        if (!request.BusinessId.Equals(Guid.Empty))
        {
            query = query.Where(x => x.BusinessId.Equals(request.BusinessId));
        }

        if (!string.IsNullOrWhiteSpace(request.RawText))
        {
            query = query.Where(x => x.RawText.Contains(request.RawText));
        }

        if (!string.IsNullOrWhiteSpace(request.SubmitedOn))
        {
            query = query.Where(x => x.SubmittedOn.Contains(request.SubmitedOn));
        }

        var entities = query.ToList();
        
        var reviews = entities.Select(x => 
            new ReviewDto(
                x.Id, 
                x.ClientId, 
                x.Rating, 
                x.RatingType.ToString(), 
                x.RawText, 
                x.SubmittedOn, 
                x.SentimentLabel, 
                x.SentimentAccuracy)
        ).ToList();

        var serializedData = JsonSerializer.Serialize(reviews);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        await cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

        return reviews;
    }
}