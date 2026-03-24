using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Clients.Commands;
using DotnetApi.Application.Clients.Validators;
using DotnetApi.Domains.Entities;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace DotnetApi.Tests.Application.Clients.Commands;

public class AddClientTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly AddClientValidator _validator;
    private readonly AddClientCommandHandler _handler;

    public AddClientTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        _validator = new AddClientValidator();
        _handler = new AddClientCommandHandler(_repositoryMock.Object, _validator);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsGuid()
    {
        // Arrange
        var command = new AddClientCommand("clientuser", "client@example.com", "+12223334444");
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = new AddClientCommand("", "invalid_email", "123");

        // Act
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}