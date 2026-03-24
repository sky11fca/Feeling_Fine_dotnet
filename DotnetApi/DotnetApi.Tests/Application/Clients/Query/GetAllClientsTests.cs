using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Clients.Query;
using DotnetApi.Domains.Entities;
using FluentAssertions;
using Moq;

namespace DotnetApi.Tests.Application.Clients.Query;

public class GetAllClientsTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly GetAllClientsQueryHandler _handler;

    public GetAllClientsTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        _handler = new GetAllClientsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsClientDtos()
    {
        // Arrange
        var client = Client.Create("client1", "client1@example.com", "+12223334444");
        var clients = new List<Client> { client }.AsQueryable();

        _repositoryMock.Setup(r => r.Query()).Returns(clients);

        var query = new GetAllClientsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Username.Should().Be("client1");
    }
}