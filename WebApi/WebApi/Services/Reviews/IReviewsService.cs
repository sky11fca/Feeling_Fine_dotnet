using WebApi.Models;

namespace WebApi.Services.Reviews;

public interface IReviewsService
{
    public Task AddReview(Guid businessId, Guid clientId, decimal review, string rawText, string submitedOn);
    public Task<List<ReviewDto?>> GetReviewQuery(Guid businessId, string rawText, string submitedOn);
}