using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Reviews.Commands;
using DotnetApi.Application.Reviews.Validators;
using DotnetApi.Domains.Entities;
using FluentAssertions;
using FluentValidation;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;

namespace DotnetApi.Tests.Application.Reviews.Commands;

public class AddReviewTests
{
    private readonly Mock<IReviewRepository> _repositoryMock;
    private readonly AddReviewValidator _validator;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly AddReviewCommandHandler _handler;

    public AddReviewTests()
    {
        _repositoryMock = new Mock<IReviewRepository>();
        _validator = new AddReviewValidator();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:8000")
        };
        _configurationMock = new Mock<IConfiguration>();
        _cacheMock = new Mock<IDistributedCache>();

        _handler = new AddReviewCommandHandler(
            _repositoryMock.Object, 
            _validator, 
            _httpClient, 
            _configurationMock.Object, 
            _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsGuid()
    {
        // Arrange
        var command = new AddReviewCommand(Guid.NewGuid(), Guid.NewGuid(), 4.5m, "Great place!", "2023-10-27");
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new { Label = "POSITIVE", Score = 0.99 })
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = new AddReviewCommand(Guid.Empty, Guid.NewGuid(), 6.0m, "", "");

        // Act
        var action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}