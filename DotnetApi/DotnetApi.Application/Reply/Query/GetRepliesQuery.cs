using MediatR;

namespace DotnetApi.Application.Reply.Query;

public record GetRepliesQuery(Guid ReviewId) : IRequest<List<ReplyDto?>>;