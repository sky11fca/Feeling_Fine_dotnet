using System.Net.Http.Json;
using WebApi.Models;
using WebApi.Models.Requests;

namespace WebApi.Services.Reply;

public class ReplyService(HttpClient client) : IReplyService
{
    private readonly string BaseUri = "http://localhost:5160/api/v1/reply";

    public async Task AddReviewAsync(Guid reviewId, Guid toClientId, string rawText)
    {
        var req = new AddReplyCommand(reviewId, toClientId, rawText);
        await client.PostAsJsonAsync(BaseUri, req);
        
    }

    public async Task<List<ReplyDto>> GetRepliesAsync(Guid reviewId)
    {
        return await client.GetFromJsonAsync<List<ReplyDto>>($"{BaseUri}?reviewId={reviewId}");
    }
}