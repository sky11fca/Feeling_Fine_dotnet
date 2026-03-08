using MediatR;

namespace DotnetApi.Application.Reply.Command;

public record AddReplyCommand(
    Guid ReviewId, 
    Guid ToClientId, 
    string RawText
    ): IRequest<Domains.Entities.Reply>;