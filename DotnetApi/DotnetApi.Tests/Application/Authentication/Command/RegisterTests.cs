using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Authentication.Command;
using DotnetApi.Application.Authentication.Validation;
using DotnetApi.Domains.Entities;
using DotnetApi.Domains.Enums;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace DotnetApi.Tests.Application.Authentication.Command;

public class RegisterTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly RegisterValidator _validator;
    private readonly RegisterCommandHandler _handler;

    public RegisterTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _validator = new RegisterValidator();
        _handler = new RegisterCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _validator);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsUser()
    {
        // Arrange
        var command = new RegisterCommand( "newuser", Guid.NewGuid(),"test@example.com", "Password123!", "Admin");
        
        _passwordHasherMock.Setup(p => p.Hash(command.Password))
            .Returns("hashed_password");
            
        _userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<Domains.Entities.User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domains.Entities.User user, CancellationToken token) => user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("newuser");
        result.Email.Should().Be("test@example.com");
        result.Password.Should().Be("hashed_password");
        result.UserRole.Should().Be(UserRole.Admin);
        
        _userRepositoryMock.Verify(r => r.AddUserAsync(It.IsAny<Domains.Entities.User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ThrowsValidationException()
    {
        // Arrange
        var command = new RegisterCommand( "newuser", Guid.NewGuid(),"invalid_email", "Password123!", "Admin");

        // Act
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
        _userRepositoryMock.Verify(r => r.AddUserAsync(It.IsAny<Domains.Entities.User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}