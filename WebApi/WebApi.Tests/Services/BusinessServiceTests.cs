using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WebApi.Models;
using WebApi.Models.Responses;
using WebApi.Services.Business;

namespace WebApi.Tests.Services
{
    public class BusinessServiceTests
    {
        private readonly Mock<IOptions<ApiSettings>> _optionsMock;

        public BusinessServiceTests()
        {
            _optionsMock = new Mock<IOptions<ApiSettings>>();
            _optionsMock.Setup(o => o.Value).Returns(new ApiSettings 
            { 
                ApiUrl = "http://localhost:5160", 
                AiUrl = "http://localhost:8000" 
            });
        }

        [Fact]
        public async Task AddBusiness_SendsPostRequest_WithCorrectUri()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var service = new BusinessService(httpClient, _optionsMock.Object);
            
            var name = "New Biz";
            var industry = "IT";
            var expectedUri = "http://localhost:5160/api/v1/business";
            var expectedGuid = Guid.NewGuid();
            var jsonResponse = JsonSerializer.Serialize(new AddBusinessResponse(expectedGuid));

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Post && 
                        req.RequestUri!.ToString().StartsWith(expectedUri)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
                });

            // Act
            await service.AddBusiness(name, industry);

            // Assert
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post && 
                    req.RequestUri!.ToString().StartsWith(expectedUri)),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task GetBusinessQuery_ReturnsBusinesses_WhenResponseIsSuccess()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var service = new BusinessService(httpClient, _optionsMock.Object);

            var name = "Test";
            var industry = "Tech";
            var expectedUri = $"http://localhost:5160/api/v1/business?name={name}&industry={industry}";
            
            var businesses = new List<BusinessDto>
            {
                new BusinessDto(Guid.NewGuid(), "Biz 1", "Tech"),
                new BusinessDto(Guid.NewGuid(), "Biz 2", "Tech")
            };
            
            var jsonResponse = JsonSerializer.Serialize(businesses);

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Get && 
                        req.RequestUri!.ToString().Contains("/api/v1/business")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
                });

            // Act
            var result = await service.GetBusinessQuery(name, industry);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Biz 1", result[0]?.Name);
            
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri!.ToString() == expectedUri),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}