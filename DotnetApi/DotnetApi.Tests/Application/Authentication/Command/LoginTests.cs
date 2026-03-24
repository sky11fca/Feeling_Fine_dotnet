using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Authentication.Command;
using DotnetApi.Application.Authentication.Validation;
using DotnetApi.Domains.Entities;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace DotnetApi.Tests.Application.Authentication.Command;

public class LoginTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly LoginValidator _validator;
    private readonly LoginCommandHandler _handler;

    public LoginTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _validator = new LoginValidator();
        _handler = new LoginCommandHandler(
            _passwordHasherMock.Object, 
            _jwtTokenGeneratorMock.Object, 
            _userRepositoryMock.Object, 
            _validator);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123!");
        var user = Domains.Entities.User.Create(Guid.NewGuid(), "test","test@example.com", "hashed_password", "Admin");

        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.Verify(command.Password, user.Password))
            .Returns(true);
        _jwtTokenGeneratorMock.Setup(j => j.GenerateToken(user))
            .Returns("jwt_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("jwt_token");
    }

    [Fact]
    public async Task Handle_InvalidEmail_ThrowsValidationException()
    {
        // Arrange
        var command = new LoginCommand("invalid_email", "Password123!");

        // Act
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsValidationException()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123!");

        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domains.Entities.User?)null);

        // Act
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await action.Should().ThrowAsync<ValidationException>();
        exception.WithMessage("*User not found*");
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsValidationException()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "WrongPassword!");
        var user = Domains.Entities.User.Create(Guid.NewGuid(), "testuser", "test@example.com", "hashed_password", "Admin");

        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.Verify(command.Password, user.Password))
            .Returns(false);

        // Act
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await action.Should().ThrowAsync<ValidationException>();
        exception.WithMessage("*Password do not mach*");
    }
}