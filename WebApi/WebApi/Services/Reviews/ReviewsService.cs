using System.Net.Http.Json;
using WebApi.Models;
using WebApi.Models.Requests;

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
        return await httpClient.GetFromJsonAsync<List<ReviewDto?>>(
            $"{BaseUri}?businessId={businessId}")!;
    }
}