using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Options;
using WebApi.Models;
using WebApi.Models.Requests;
using WebApi.Services.Authentication;
using Xunit;
using System;

namespace WebApi.Tests.Services
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<IOptions<ApiSettings>> _optionsMock;

        public AuthenticationServiceTests()
        {
            _optionsMock = new Mock<IOptions<ApiSettings>>();
            _optionsMock.Setup(o => o.Value).Returns(new ApiSettings 
            { 
                ApiUrl = "http://localhost:5160", 
                AiUrl = "http://localhost:8000" 
            });
        }

        [Fact]
        public async Task Login_SendsPostRequest_WithCorrectUri()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var service = new AuthenticationService(httpClient, _optionsMock.Object);
            
            var email = "test@test.com";
            var password = "password";
            var expectedUri = "http://localhost:5160/api/Authentication/login";

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Post && 
                        req.RequestUri!.ToString() == expectedUri),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("fake-token")
                });

            // Act
            var result = await service.Login(email, password);

            // Assert
            Assert.Equal("fake-token", result);
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
        public async Task Register_SendsPostRequest_WithCorrectUri()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            var service = new AuthenticationService(httpClient, _optionsMock.Object);
            
            var command = new RegisterCommand
            {
                Email = "test@test.com",
                Password = "password",
                Username = "testuser"
            };
            var expectedUri = "http://localhost:5160/api/Authentication/register";
            var userDto = new UserDto(Guid.NewGuid(), Guid.NewGuid(), "testuser", "test@test.com", "password", 1, DateTime.UtcNow);

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Post && 
                        req.RequestUri!.ToString() == expectedUri),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(userDto))
                });

            // Act
            var result = await service.Register(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto.Email, result.Email);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post && 
                    req.RequestUri!.ToString() == expectedUri),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}