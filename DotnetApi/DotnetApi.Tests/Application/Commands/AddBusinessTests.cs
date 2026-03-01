using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Businesses.Commands;
using DotnetApi.Domains.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace DotnetApi.Tests.Application.Commands;

public class AddBusinessTests : IDisposable
{
    private readonly Mock<IBusinessRepository> _repositoryMock;
    private readonly Mock<IValidator<AddBusinessCommand>> _validatorMock;
    private readonly AddBusinessCommandHandler _handler;

    public AddBusinessTests()
    {
        _repositoryMock = new Mock<IBusinessRepository>() ;
        _validatorMock = new Mock<IValidator<AddBusinessCommand>>();
        _handler = new AddBusinessCommandHandler(_repositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task GivenInvalidRequestWhenCreatingBusinessThenReturnsError()
    {
        // Arrange
        var command = new AddBusinessCommand("", "");
        var validationFailure = new ValidationFailure("Name", "Name is required");
        var validationResult = new ValidationResult(new[] { validationFailure });
 
        _validatorMock
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
 
        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);
 
        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Business>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]

    public async Task GivenValidRequestWhenCreatingBusinessThenReturnsBusinessId()
    {
        // Arrange
        // Assuming AddBusinessCommand has a constructor or init properties matching the usage in the handler
        var command = new AddBusinessCommand("Test Business", "Technology");
         
        _validatorMock
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult()); // Valid result (IsValid = true)
 
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
 
        // Assert
        result.Should().NotBeEmpty();
         
        _repositoryMock.Verify(x => x.AddAsync(
                It.Is<Business>(b => b.Name == command.Name && b.Industry == command.Industry), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    public void Dispose()
    {
        
    }
}