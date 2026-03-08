using DotnetApi.Application.Reviews.Commands;
using DotnetApi.Application.Reviews.Queries;
using DotnetApi.Domains.Enums;
using DotnetApi.WebApi.Controller;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Dotnet.Tests.Controllers;

public class ReviewControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ReviewController _controller;

    public ReviewControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ReviewController(_mediatorMock.Object);
    }

    [Fact]
    public async Task AddReview_ShouldReturnCreated_WhenCommandIsSuccessful()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var command = new AddReviewCommand(expectedId, Guid.NewGuid(),3.5m, "test", "2023-10-27");

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _controller.AddReview(command);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal($"/api/v1/review/{expectedId}", createdResult.Location);

        var value = createdResult.Value;
        Assert.NotNull(value);
        
        // Use reflection to access the anonymous type property 'id'
        var idProperty = value!.GetType().GetProperty("id");
        Assert.NotNull(idProperty);
        var actualId = idProperty!.GetValue(value);
        Assert.Equal(expectedId, actualId);

        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetReview_ShouldReturnOk_WhenQueryIsSuccessful()
    {
        // Arrange
        var businessId = Guid.NewGuid();
        var rawText = "test";
        var submitedOn = "2023-10-27";
        var cancellationToken = new CancellationToken();
        var expectedResult = new List<ReviewDto> { new ReviewDto(businessId, Guid.NewGuid(),3.5m, "Mixed", rawText, submitedOn, "Unknown", 0.05)};

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetReviewQuery>(), cancellationToken))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetReview(businessId, rawText, submitedOn, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expectedResult, okResult.Value);

        _mediatorMock.Verify(m => m.Send(It.Is<GetReviewQuery>(q => q.BusinessId == businessId && q.RawText == rawText && q.SubmitedOn == submitedOn), cancellationToken), Times.Once);
    }
}