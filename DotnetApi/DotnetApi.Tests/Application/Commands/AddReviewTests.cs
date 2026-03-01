using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Businesses.Commands;
using DotnetApi.Application.Reviews.Commands;
using DotnetApi.Application.Reviews.Validators;
using DotnetApi.Domains.Entities;
using DotnetApi.Tests.Helpers;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace DotnetApi.Tests.Application.Commands;

public class AddReviewTests : IDisposable
{
    private readonly Mock<IReviewRepository> _repositoryMock;
    private readonly Mock<IValidator<AddReviewCommand>> _validatorMock;
    private readonly AddReviewCommandHandler _handler;

    public AddReviewTests()
    {
        _repositoryMock = new Mock<IReviewRepository>();
        _validatorMock = new Mock<IValidator<AddReviewCommand>>();
        _handler = new AddReviewCommandHandler(_repositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task GivenInvalidRequestWhenCreatingNewReviewThenReturnsError()
    {
        var command = new AddReviewCommand(Guid.Empty, -6.7m, "testing", "testing");
        var validationFailure = new ValidationFailure("BusinessId", "Must not be empty");
        var validationResult = new ValidationResult(new[] { validationFailure });
        
        
        _validatorMock
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var act = async () => await _handler.Handle(command, It.IsAny<CancellationToken>());
        
        await act.Should().ThrowAsync<ValidationException>();
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GivenValidRequestWhenCreatingNewReviewThenReturnsId()
    {
        var command = new AddReviewCommand(Guid.NewGuid(), 5.0m, "testing", "test");
         
        _validatorMock
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult()); // Valid result (IsValid = true)
 
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
 
        // Assert
        result.Should().NotBeEmpty();
         
        _repositoryMock.Verify(x => x.AddAsync(
                It.Is<Review>(r => r.BusinessId == command.BusinessId && r.Rating == command.Review && r.RawText == command.RawText), 
                It.IsAny<CancellationToken>()), 
            Times.Once);

    }
    
    
    public void Dispose()
    {
        
    }
}