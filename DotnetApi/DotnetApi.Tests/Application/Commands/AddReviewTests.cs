using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Reviews.Commands;
using DotnetApi.Application.Reviews.Validators;
using DotnetApi.Tests.Helpers;
using Moq;

namespace DotnetApi.Tests.Application.Commands;

public class AddReviewTests : IDisposable
{

    [Fact]
    public async Task GivenInvalidRequestWhenCreatingNewReviewThenReturnsError()
    {
        //Arrange
        var mock = new Mock<IReviewRepository>();
        var validator = new AddReviewValidator();
        var handler = new AddReviewCommandHandler(mock.Object, validator);
        var command = new AddReviewCommand(Guid.NewGuid(), -1m, string.Empty, string.Empty);

        //Act
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task GivenValidRequestWhenCreatingNewReviewThenReturnsId()
    {
        var mock = new Mock<IReviewRepository>();
        var validator = new AddReviewValidator();
        var handler = new AddReviewCommandHandler(mock.Object, validator);
        var command = new AddReviewCommand(Guid.NewGuid(), 4.5m, "testing", "testing");
        
        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.NotEqual(Guid.Empty, result);

    }
    
    
    public void Dispose()
    {
        
    }
}