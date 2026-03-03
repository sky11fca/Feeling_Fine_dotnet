using DotnetApi.Application.Businesses.Commands;
using DotnetApi.Application.Businesses.Queries;
using DotnetApi.WebApi.Controller;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DotnetApi.Tests.Controller;

public class BusinessControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly BusinessController _controller;

    public BusinessControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new BusinessController(_mediatorMock.Object);
    }

    [Fact]
    public async Task AddBusiness_ShouldReturnCreated_WhenCommandIsSuccessful()
    {
        // Arrange
        var command = new AddBusinessCommand("test", "test"); // Assuming default constructor exists
        var expectedId = Guid.NewGuid(); // Assuming ID is a Guid, adjust if int

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _controller.AddBusiness(command, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal($"/api/v1/business/{expectedId}", createdResult.Location);
        
        // Verify the anonymous type property using reflection or dynamic
        var value = createdResult.Value;
        Assert.NotNull(value);
        var idProperty = value.GetType().GetProperty("id")?.GetValue(value, null);
        Assert.Equal(expectedId, idProperty);
    }

    [Fact]
    public async Task GetBusiness_ShouldReturnOk_WhenQueryIsSuccessful()
    {
        // Arrange
        var businessId = Guid.NewGuid();
        var name = "Test Business";
        var industry = "Tech";
        var expectedResult = new List<BusinessDto> { new BusinessDto(businessId, name, industry) }; // Mocking the result object

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetBusinessQuery>(q => q.Name == name && q.Industry == industry), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetBusiness(name, industry, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Same(expectedResult, okResult.Value);
    }
}