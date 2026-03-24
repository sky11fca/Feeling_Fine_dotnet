using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Reply.Query;
using FluentAssertions;
using Moq;

namespace DotnetApi.Tests.Application.Reply.Query;

public class GetRepliesTests
{
    private readonly Mock<IReplyRepository> _repositoryMock;
    private readonly GetRepliesQueryHandler _handler;

    public GetRepliesTests()
    {
        _repositoryMock = new Mock<IReplyRepository>();
        _handler = new GetRepliesQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsReplyDtos()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var reply = Domains.Entities.Reply.Create(reviewId, Guid.NewGuid(), "Thanks for your review!");
        var replies = new List<Domains.Entities.Reply> { reply }.AsQueryable();

        _repositoryMock.Setup(r => r.Query()).Returns(replies);

        var query = new GetRepliesQuery(reviewId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First()!.RawText.Should().Be("Thanks for your review!");
    }
}