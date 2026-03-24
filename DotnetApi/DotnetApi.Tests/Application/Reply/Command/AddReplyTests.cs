using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Reply.Command;
using DotnetApi.Application.Reply.Validator;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace DotnetApi.Tests.Application.Reply.Command;

public class AddReplyTests
{
    private readonly Mock<IReplyRepository> _repositoryMock;
    private readonly AddReplyValidator _validator;
    private readonly AddReplyCommandHandler _handler;

    public AddReplyTests()
    {
        _repositoryMock = new Mock<IReplyRepository>();
        _validator = new AddReplyValidator();
        _handler = new AddReplyCommandHandler(_repositoryMock.Object, _validator);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsReply()
    {
        // Arrange
        var command = new AddReplyCommand(Guid.NewGuid(), Guid.NewGuid(), "Thanks for your feedback!");
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Domains.Entities.Reply>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.RawText.Should().Be("Thanks for your feedback!");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domains.Entities.Reply>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = new AddReplyCommand(Guid.Empty, Guid.Empty, "");

        // Act
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domains.Entities.Reply>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}