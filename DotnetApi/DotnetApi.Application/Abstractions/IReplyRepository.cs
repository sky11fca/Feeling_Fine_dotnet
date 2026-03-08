using DotnetApi.Domains.Entities;

namespace DotnetApi.Application.Abstractions;

public interface IReplyRepository
{
    Task AddAsync(Domains.Entities.Reply reply, CancellationToken cancellationToken = default);
    Task<Domains.Entities.Reply?> FindReply(Guid id, CancellationToken cancellationToken = default);
    IQueryable<Domains.Entities.Reply> Query();
    
}