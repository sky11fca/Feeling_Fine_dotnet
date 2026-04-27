using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Options;
using WebApi.Models;
using WebApi.Services.Client;
using Xunit;
using System;
using System.Collections.Generic;

namespace WebApi.Tests.Services
{
    public class ClientServiceTests
    {
        private readonly Mock<IOptions<ApiSettings>> _optionsMock;

        public ClientServiceTests()
        {
            _optionsMock = new Mock<IOptions<ApiSettings>>();
            _optionsMock.Setup(x => x.Value).Returns(new ApiSettings
            {
                ApiUrl = "http://localhost:5160",
                AiUrl = "http://localhost:8000"
            });
        }

        [Fact]
        public async Task AddAsync_SendsPostRequest_WithCorrectUri()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var service = new ClientService(httpClient, _optionsMock.Object);
            
            var username = "testuser";
            var email = "test@test.com";
            var phoneNumber = "1234567890";
            var expectedUri = "http://localhost:5160/api/v1/client";

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
            await service.AddAsync(username, email, phoneNumber);

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
        public async Task FindAsync_ReturnsClient_WhenResponseIsSuccess()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var service = new ClientService(httpClient, _optionsMock.Object);

            var clientId = Guid.NewGuid();
            var expectedUri = $"http://localhost:5160/api/v1/client/{clientId}";
            
            var clientDto = new ClientDto(clientId, "testuser", "test@test.com", "1234567890");
            
            var jsonResponse = JsonSerializer.Serialize(clientDto);

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
            var result = await service.FindAsync(clientId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(clientId, result.Id);
            
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task Query_ReturnsClients_WhenResponseIsSuccess()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var service = new ClientService(httpClient, _optionsMock.Object);

            var expectedUri = "http://localhost:5160/api/v1/client";
            
            var clients = new List<ClientDto>
            {
                new ClientDto(Guid.NewGuid(), "user1", "user1@test.com", "111"),
                new ClientDto(Guid.NewGuid(), "user2", "user2@test.com", "222")
            };
            
            var jsonResponse = JsonSerializer.Serialize(clients);

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
            var result = await service.Query();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}