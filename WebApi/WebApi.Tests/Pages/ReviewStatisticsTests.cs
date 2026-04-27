using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using WebApi.Models;
using WebApi.Models.Responses;
using WebApi.Pages;
using WebApi.Services.Client;
using WebApi.Services.Reviews;
using WebApi.Shared;

namespace WebApi.Tests.Pages
{
    public class ReviewStatisticsTests : BunitContext, IAsyncLifetime
    {
        public ReviewStatisticsTests()
        {
            JSInterop.Mode = JSRuntimeMode.Loose;
            Services.AddMudServices();
            ComponentFactories.AddStub<Navbar>();
        }

        [Fact]
        public void Redirects_To_Login_When_Token_Missing()
        {
            // Arrange
            JSInterop.Setup<string>("localStorage.getItem", _ => true).SetResult(string.Empty);
            
            var mockReviewsService = new Mock<IReviewsService>();
            var mockClientService = new Mock<IClientService>();

            Services.AddSingleton(mockReviewsService.Object);
            Services.AddSingleton(mockClientService.Object);

            var navMan = Services.GetRequiredService<NavigationManager>();

            // Act
            Render<ReviewStatistics>();

            // Assert
            Assert.EndsWith("/login", navMan.Uri);
        }

        [Fact]
        public void Renders_Statistics_When_Data_Exists()
        {
            // Arrange
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult("token");
            JSInterop.Setup<string>("localStorage.getItem", "businessId").SetResult(System.Guid.NewGuid().ToString());
            
            var clientId1 = Guid.NewGuid();
            var clientId2 = Guid.NewGuid();
            
            var reviews = new List<ReviewDto?>
            {
                new ReviewDto (Guid.NewGuid(), clientId1, 5.0m, "Great!", "TEXT1", "Positive", "1.0", 0.999),
                new ReviewDto (Guid.NewGuid(), clientId2, 1.0m, "Bad!", "TEXT2", "Negative", "1.0", 0.999),
                new ReviewDto (Guid.NewGuid(), clientId1, 4.0m, "Good!", "TEXT3", "Positive", "1.0", 0.999)
            };

            var mockReviewsService = new Mock<IReviewsService>();
            mockReviewsService.Setup(s => s.GetReviewQuery(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(reviews);

            var aiStats = new AiStatisticsResponse
            {
                Ratings = new ChartData { Labels = new[] { "5", "1", "4" }, Data = new[] { 1, 1, 1 } },
                Sentiments = new ChartData { Labels = new[] { "Positive", "Negative" }, Data = new[] { 2, 1 } },
                Clients = new ChartData { Labels = new[] { "User1", "User2" }, Data = new[] { 2, 1 } }
            };
            mockReviewsService.Setup(s => s.GetAiStatistics(It.IsAny<List<ReviewDto?>>())).ReturnsAsync(aiStats);

            var mockClientService = new Mock<IClientService>();
            mockClientService.Setup(s => s.FindAsync(clientId1)).ReturnsAsync(new ClientDto(clientId1, "User1", "e1", "123"));
            mockClientService.Setup(s => s.FindAsync(clientId2)).ReturnsAsync(new ClientDto(clientId2, "User2", "e2", "123"));

            Services.AddSingleton(mockReviewsService.Object);
            Services.AddSingleton(mockClientService.Object);

            // Act
            var cut = Render<ReviewStatistics>();
            
            // Assert
            Assert.Contains("Successfully analysed 3 reviews", cut.Markup);
            Assert.Contains("Reviews per Category", cut.Markup);
            Assert.Contains("Top Clients", cut.Markup);
            Assert.Contains("Sentiment Analysis", cut.Markup);
            Assert.Contains("Export CSV", cut.Markup);
        }

        [Fact]
        public void Shows_Info_Alerts_When_No_Data_Available()
        {
            // Arrange
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult("token");
            JSInterop.Setup<string>("localStorage.getItem", "businessId").SetResult(Guid.NewGuid().ToString());

            var mockReviewsService = new Mock<IReviewsService>();
            mockReviewsService.Setup(s => s.GetReviewQuery(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<ReviewDto?>());

            var mockClientService = new Mock<IClientService>();

            Services.AddSingleton(mockReviewsService.Object);
            Services.AddSingleton(mockClientService.Object);

            // Act
            var cut = Render<ReviewStatistics>();

            // Assert
            Assert.Contains("No reviews found to analyze.", cut.Markup);
        }

        [Fact]
        public void Shows_Error_Message_On_Exception()
        {
            // Arrange
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult("token");
            JSInterop.Setup<string>("localStorage.getItem", "businessId").SetResult(Guid.NewGuid().ToString());

            var mockReviewsService = new Mock<IReviewsService>();
            mockReviewsService.Setup(s => s.GetReviewQuery(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("API failure"));

            var mockClientService = new Mock<IClientService>();

            Services.AddSingleton(mockReviewsService.Object);
            Services.AddSingleton(mockClientService.Object);

            // Act
            var cut = Render<ReviewStatistics>();

            // Assert
            Assert.Contains("Critical error: API failure", cut.Markup);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        Task IAsyncLifetime.DisposeAsync()
        {
            if (this is IAsyncDisposable asyncDisposable)
            {
                return asyncDisposable.DisposeAsync().AsTask();
            }
            return Task.CompletedTask;
        }
    }
}