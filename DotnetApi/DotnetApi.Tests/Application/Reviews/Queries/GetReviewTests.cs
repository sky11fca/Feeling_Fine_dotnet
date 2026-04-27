using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Reviews.Queries;
using DotnetApi.Domains.Entities;
using DotnetApi.Domains.Enums;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace DotnetApi.Tests.Application.Reviews.Queries;

public class GetReviewTests
{
    private readonly Mock<IReviewRepository> _repositoryMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly GetReviewQueryHandler _handler;

    public GetReviewTests()
    {
        _repositoryMock = new Mock<IReviewRepository>();
        _cacheMock = new Mock<IDistributedCache>();
        _handler = new GetReviewQueryHandler(_repositoryMock.Object, _cacheMock.Object);
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
        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null!);

        var query = new GetReviewQuery(businessId, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().RawText.Should().Be("Good");
    }

    [Fact]
    public async Task Handle_ReturnsCachedData_WhenCacheExists()
    {
        // Arrange
        var businessId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var reviews = new List<ReviewDto> 
        { 
            new ReviewDto(Guid.NewGuid(), clientId, 4.0m, "MostlyPositive", "Cached Review", "2023-10-27", "POSITIVE", 0.95) 
        };
        var serialized = JsonSerializer.Serialize(reviews);
        var cachedBytes = Encoding.UTF8.GetBytes(serialized);

        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedBytes);

        var query = new GetReviewQuery(businessId, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().RawText.Should().Be("Cached Review");
        _repositoryMock.Verify(r => r.Query(), Times.Never);
    }
}