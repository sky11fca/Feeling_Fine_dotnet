using DotnetApi.Domains.Entities;
using DotnetApi.Infrastructure.Authentication;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace DotnetApi.Tests.Infrastructure.Authentication;

public class JwtTokenGeneratorTests
{
    private readonly Mock<IOptions<JwtSettings>> _optionsMock;

    public JwtTokenGeneratorTests()
    {
        _optionsMock = new Mock<IOptions<JwtSettings>>();
    }

    [Fact]
    public void GenerateToken_ValidSettings_ReturnsToken()
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = "a_very_long_secret_key_for_testing_purposes_1234567890",
            Issuer = "FeelingFine",
            Audience = "FeelingFine"
        };
        _optionsMock.Setup(o => o.Value).Returns(settings);
        var generator = new JwtTokenGenerator(_optionsMock.Object);
        var user = User.Create(Guid.NewGuid(), "testuser", "test@example.com", "hashed", "Admin");

        // Act
        var token = generator.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateToken_MissingSecret_ThrowsException()
    {
        // Arrange
        var settings = new JwtSettings
        {
            Secret = string.Empty,
            Issuer = "FeelingFine",
            Audience = "FeelingFine"
        };
        _optionsMock.Setup(o => o.Value).Returns(settings);
        var generator = new JwtTokenGenerator(_optionsMock.Object);
        var user = User.Create(Guid.NewGuid(), "testuser", "test@example.com", "hashed", "Admin");

        // Act
        Action action = () => generator.GenerateToken(user);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("JWT Secret is not configured.");
    }
}
