using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using WebApi.Models;
using WebApi.Services.Reply;
using Xunit;
using System;
using System.Collections.Generic;

namespace WebApi.Tests.Services
{
    public class ReplyServiceTests
    {
        [Fact]
        public async Task AddReviewAsync_SendsPostRequest_WithCorrectUri()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var service = new ReplyService(httpClient);
            
            var reviewId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var rawText = "Thank you for the feedback!";
            var expectedUri = "http://localhost:5160/api/v1/reply";

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
            await service.AddReviewAsync(reviewId, clientId, rawText);

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
        public async Task GetRepliesAsync_ReturnsReplies_WhenResponseIsSuccess()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var service = new ReplyService(httpClient);

            var reviewId = Guid.NewGuid();
            var expectedUri = $"http://localhost:5160/api/v1/reply?reviewId={reviewId}";
            
            var replies = new List<ReplyDto>
            {
                new ReplyDto(reviewId,  Guid.NewGuid(), "Thank you!"),
                new ReplyDto(reviewId, Guid.NewGuid(), "We will do better!")
            };
            
            var jsonResponse = JsonSerializer.Serialize(replies);

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
            var result = await service.GetRepliesAsync(reviewId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Thank you!", result[0].RawText);
            
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}