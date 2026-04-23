using System.Net.Http.Json;
using WebApi.Models;
using WebApi.Models.Requests;
using WebApi.Models.Responses;

namespace WebApi.Services.Reviews;

public class ReviewsService(HttpClient httpClient) : IReviewsService
{

    private readonly string BaseUri = "http://localhost:5160/api/v1/review";
    public async Task AddReview(Guid businessId, Guid clientId, decimal review, string rawText, string submitedOn)
    {
        var request = new AddReviewCommand(businessId, clientId, review, rawText, submitedOn);
        await httpClient.PostAsJsonAsync(BaseUri, request);
    }

    public async Task<List<ReviewDto?>> GetReviewQuery(Guid businessId, string rawText, string submitedOn)
    {
        var fullUrl = businessId != Guid.Empty  ? $"{BaseUri}?businessId={businessId}" : BaseUri;
        return await httpClient.GetFromJsonAsync<List<ReviewDto?>>(
            fullUrl)!;
    }

    public async Task<AiStatisticsResponse?> GetAiStatistics(List<ReviewDto?> reviews)
    {
        var aiBaseUri = "http://localhost:8000/ai/statistics/";
        var validReviews = reviews.Where(r => r != null).Select(r => new ReviewAiModel
        {
            RawText = r!.RawText,
            SubmittedOn = r.SubmittedOn,
            Rating = (double)r.Review,
            ClientId = r.ClientId.ToString(),
            RatingType = r.RatingType,
            SentimentLabel = r.SentimentLabel
        }).ToList();

        var response = await httpClient.PostAsJsonAsync(aiBaseUri, validReviews);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AiStatisticsResponse>();
        }
        return null;
    }
}