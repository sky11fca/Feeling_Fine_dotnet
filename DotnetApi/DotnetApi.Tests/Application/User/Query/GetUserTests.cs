using DotnetApi.Application.Abstractions;
using DotnetApi.Application.User.Query;
using DotnetApi.Domains.Entities;
using FluentAssertions;
using Moq;

namespace DotnetApi.Tests.Application.User.Query;

public class GetUserTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly GetUserQueryHandler _handler;

    public GetUserTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new GetUserQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsUsers()
    {
        // Arrange
        var user = Domains.Entities.User.Create(Guid.NewGuid(), "john_doe", "john@example.com", "password123", "Admin");
        var users = new List<Domains.Entities.User?> { user };

        _repositoryMock.Setup(r => r.GetUsersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var query = new GetUserQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First()!.Username.Should().Be("john_doe");
    }
}