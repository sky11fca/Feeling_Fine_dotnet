using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Reviews.Commands;
using DotnetApi.Application.Reviews.Validators;
using DotnetApi.Domains.Entities;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace DotnetApi.Tests.Application.Reviews.Commands;

public class AddReviewTests
{
    private readonly Mock<IReviewRepository> _repositoryMock;
    private readonly AddReviewValidator _validator;
    private readonly AddReviewCommandHandler _handler;

    public AddReviewTests()
    {
        _repositoryMock = new Mock<IReviewRepository>();
        _validator = new AddReviewValidator();
        _handler = new AddReviewCommandHandler(_repositoryMock.Object, _validator);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsGuid()
    {
        // Arrange
        var command = new AddReviewCommand(Guid.NewGuid(), Guid.NewGuid(), 4.5m, "Great place!", "2023-10-27");
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = new AddReviewCommand(Guid.Empty, Guid.NewGuid(), 6.0m, "", "");

        // Act
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}