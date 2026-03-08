using WebApi.Models;

namespace WebApi.Services.Reply;

public interface IReplyService
{
    Task AddReviewAsync(Guid reviewId, Guid toClientId, string rawText);
    Task<List<ReplyDto>> GetRepliesAsync(Guid reviewId);
}