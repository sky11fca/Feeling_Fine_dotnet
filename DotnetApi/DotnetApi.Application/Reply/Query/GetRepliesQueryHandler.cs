using DotnetApi.Application.Abstractions;
using MediatR;

namespace DotnetApi.Application.Reply.Query;

public class GetRepliesQueryHandler(IReplyRepository repository) : IRequestHandler<GetRepliesQuery, List<ReplyDto?>>
{
    public Task<List<ReplyDto?>> Handle(GetRepliesQuery request, CancellationToken cancellationToken)
    {
        var query = repository
            .Query()
            .Where(x => x.ReviewId == request.ReviewId)
            .Select(x => new ReplyDto(x.ReviewId, x.ToClientId, x.RawText))
            .ToList();

        return Task.FromResult(query);
    }
}