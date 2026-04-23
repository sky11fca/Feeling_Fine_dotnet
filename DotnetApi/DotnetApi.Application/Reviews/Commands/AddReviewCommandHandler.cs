using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Reviews.Queries;
using DotnetApi.Domains.Entities;
using DotnetApi.Domains.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using System.Net.Http.Json;

namespace DotnetApi.Application.Reviews.Commands;

public class AddReviewCommandHandler(IReviewRepository repository, IValidator<AddReviewCommand> validator, HttpClient httpClient, IConfiguration configuration, IDistributedCache cache) : IRequestHandler<AddReviewCommand, Guid>
{
    public async Task<Guid> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        // 1. Determine Rating Type
        var finalReviewType = request.Review switch
        {
            >= 5.0m => RatingType.OverwhelminglyPositive,
            >= 4.0m => RatingType.MostlyPositive,
            >= 3.0m => RatingType.Mixed,
            >= 2.0m => RatingType.MostlyNegative,
            _ => RatingType.OverwhelminglyNegative
        };

        // 2. Perform Sentiment Analysis
        var aiServiceUrl = configuration["AiServiceUrl"] ?? "http://localhost:8000/ai/review/";
        SentimentAnalysisResult? sentiment = null;
        try
        {
            var response = await httpClient.PostAsJsonAsync(aiServiceUrl, new { raw_text = request.RawText, submitted_on = request.SubmitedOn }, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                sentiment = await response.Content.ReadFromJsonAsync<SentimentAnalysisResult>(cancellationToken: cancellationToken);
            }
        }
        catch { /* Fallback to defaults if AI is down */ }

        // 3. Create and Persist with Sentiment
        var review = Review.Create(
            request.BusinessId, 
            request.ClientId, 
            request.Review, 
            finalReviewType, 
            request.RawText, 
            request.SubmitedOn,
            sentiment?.Label ?? "Unknown",
            sentiment?.Score ?? 0);

        await repository.AddAsync(review, cancellationToken);

        // 4. Invalidate Cache
        // Invalidate the most likely cache key (empty filters)
        string cacheKey = $"reviews_{request.BusinessId}__";
        await cache.RemoveAsync(cacheKey, cancellationToken);
        // Also invalidate the 'all reviews' cache for admins
        await cache.RemoveAsync($"reviews_{Guid.Empty}__", cancellationToken);

        return review.Id;
    }
}