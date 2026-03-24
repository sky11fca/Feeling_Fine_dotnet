using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Clients.Query;
using DotnetApi.Domains.Entities;
using FluentAssertions;
using Moq;

namespace DotnetApi.Tests.Application.Clients.Query;

public class GetClientByIdTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly GetClientByIdQueryHandler _handler;

    public GetClientByIdTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        _handler = new GetClientByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsClientDto()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var client = Client.Create("client1", "client1@example.com", "+12223334444");
        
        // Use reflection to set the Id property since it has a private setter
        var idProperty = client.GetType().GetProperty("Id");
        idProperty?.SetValue(client, clientId);

        _repositoryMock.Setup(r => r.FindAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var query = new GetClientByIdQuery(clientId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(clientId);
        result.Username.Should().Be("client1");
    }

    [Fact]
    public async Task Handle_NonExistingId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var query = new GetClientByIdQuery(Guid.NewGuid());

        _repositoryMock.Setup(r => r.FindAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        // Act
        var action = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<KeyNotFoundException>();
    }
}