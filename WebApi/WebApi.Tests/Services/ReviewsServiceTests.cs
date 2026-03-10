using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using WebApi.Models;
using WebApi.Services.Reviews;
using Xunit;

namespace WebApi.Tests.Services
{
    public class ReviewsServiceTests
    {
        [Fact]
        public async Task AddReview_SendsPostRequest_WithCorrectUri()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var service = new ReviewsService(httpClient);
            
            var businessId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var review = 5.0m;
            var rawText = "Great service";
            var submittedOn = "2023-10-27";
            var expectedUri = "http://localhost:5160/api/v1/review";

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Post && 
                        req.RequestUri!.ToString() == expectedUri),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await service.AddReview(businessId, clientId, review, rawText, submittedOn);

            // Assert
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post && 
                    req.RequestUri!.ToString() == expectedUri),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task GetReviewQuery_ReturnsReviews_WhenResponseIsSuccess()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var service = new ReviewsService(httpClient);

            var businessId = Guid.NewGuid();
            var clientId1 = Guid.NewGuid();
            var clientId2 = Guid.NewGuid();
            // Note: The service implementation currently only uses businessId in the query string
            var expectedUri = $"http://localhost:5160/api/v1/review?businessId={businessId}";
            
            var reviews = new List<ReviewDto>
            {
                new ReviewDto(Guid.NewGuid(), clientId1,5.0m, "Excellent", "Great!", "2023-01-01", "Positive", 0.95),
                new ReviewDto(Guid.NewGuid(), clientId2, 1.0m, "Poor", "Bad!", "2023-01-02", "Negative", 0.85)
            };
            
            var jsonResponse = JsonSerializer.Serialize(reviews);

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Get && 
                        req.RequestUri!.ToString() == expectedUri),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var result = await service.GetReviewQuery(businessId, string.Empty, string.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Great!", result[0]?.RawText);
            
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}