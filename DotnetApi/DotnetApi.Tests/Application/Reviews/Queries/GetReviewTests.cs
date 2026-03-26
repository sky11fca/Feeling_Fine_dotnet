using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Reviews.Queries;
using DotnetApi.Domains.Entities;
using DotnetApi.Domains.Enums;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;

namespace DotnetApi.Tests.Application.Reviews.Queries;

public class GetReviewTests
{
    private readonly Mock<IReviewRepository> _repositoryMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly GetReviewQueryHandler _handler;

    public GetReviewTests()
    {
        _repositoryMock = new Mock<IReviewRepository>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:8000")
        };
        _configurationMock = new Mock<IConfiguration>();
        _handler = new GetReviewQueryHandler(_repositoryMock.Object, _httpClient, _configurationMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsReviewDtos()
    {
        // Arrange
        var businessId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var review = Review.Create(businessId, clientId, 4.0m, RatingType.MostlyPositive, "Good", "2023-10-27");
        var reviews = new List<Review> { review }.AsQueryable();

        _repositoryMock.Setup(r => r.Query()).Returns(reviews);

        var sentimentResponse = new SentimentAnalysisResult("POSITIVE", 0.95);
        
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(sentimentResponse)
            });

        var query = new GetReviewQuery(businessId, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().RawText.Should().Be("Good");
        result.First().SentimentLabel.Should().Be("POSITIVE");
    }

    [Fact]
    public async Task Handle_AiServiceFails_ReturnsReviewDtosWithUnknownSentiment()
    {
        // Arrange
        var businessId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var review = Review.Create(businessId, clientId, 4.0m, RatingType.MostlyPositive, "Good", "2023-10-27");
        var reviews = new List<Review> { review }.AsQueryable();

        _repositoryMock.Setup(r => r.Query()).Returns(reviews);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException());

        var query = new GetReviewQuery(businessId, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().RawText.Should().Be("Good");
        result.First().SentimentLabel.Should().Be("Unknown");
    }
}