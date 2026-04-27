using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Services;
using WebApi.Models;
using WebApi.Models.Responses;
using WebApi.Pages;
using WebApi.Services.Client;
using WebApi.Services.Reviews;
using Microsoft.JSInterop;

namespace WebApi.Tests.Pages
{
    public class ReviewStatisticsTests : BunitContext
    {
        public ReviewStatisticsTests()
        {
            Services.AddMudServices();
            JSInterop.Mode = JSRuntimeMode.Loose;
            
            // Mock JSInterop calls
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult("dummy-token");
            JSInterop.Setup<string>("localStorage.getItem", "businessId").SetResult(Guid.NewGuid().ToString());
            
            ComponentFactories.AddStub<WebApi.Shared.Navbar>();
        }

        [Fact]
        public void ReviewStatistics_Initialization_LoadsData()
        {
            // Arrange
            var mockReviewsService = new Mock<IReviewsService>();
            var mockClientService = new Mock<IClientService>();
            
            var reviews = new List<ReviewDto?> { 
                new ReviewDto(Guid.NewGuid(), Guid.NewGuid(), 5m, "Good", "Text", "2023-10-27", "Positive", 0.9) 
            };
            
            var stats = new AiStatisticsResponse
            {
                Ratings = new ChartData { Labels = new[] { "5" }, Data = new[] { 1 } },
                Sentiments = new ChartData { Labels = new[] { "Positive" }, Data = new[] { 1 } },
                Clients = new ChartData { Labels = new[] { "User" }, Data = new[] { 1 } }
            };

            mockReviewsService.Setup(s => s.GetReviewQuery(It.IsAny<Guid>(), "", "")).ReturnsAsync(reviews);
            mockReviewsService.Setup(s => s.GetAiStatistics(It.IsAny<List<ReviewDto?>>())).ReturnsAsync(stats);

            Services.AddSingleton(mockReviewsService.Object);
            Services.AddSingleton(mockClientService.Object);

            // Act
            var cut = Render<ReviewStatistics>();
            cut.WaitForState(() => !cut.Markup.Contains("MudProgressCircular"));

            // Assert
            Assert.Contains("Successfully analysed 1 reviews", cut.Markup);
            mockReviewsService.Verify(s => s.GetAiStatistics(It.IsAny<List<ReviewDto?>>()), Times.Once);
        }

        [Fact]
        public async Task ReviewStatistics_ExportCsv_Detailed_CallsJS()
        {
            // Arrange
            var mockReviewsService = new Mock<IReviewsService>();
            var mockClientService = new Mock<IClientService>();
            
            var reviews = new List<ReviewDto?> { 
                new ReviewDto(Guid.NewGuid(), Guid.NewGuid(), 5m, "Good", "Text", "2023-10-27", "Positive", 0.9) 
            };
            
            var stats = new AiStatisticsResponse
            {
                Ratings = new ChartData { Labels = new[] { "5" }, Data = new[] { 1 } },
                Sentiments = new ChartData { Labels = new[] { "Positive" }, Data = new[] { 1 } },
                Clients = new ChartData { Labels = new[] { "User" }, Data = new[] { 1 } }
            };

            mockReviewsService.Setup(s => s.GetReviewQuery(It.IsAny<Guid>(), "", "")).ReturnsAsync(reviews);
            mockReviewsService.Setup(s => s.GetAiStatistics(It.IsAny<List<ReviewDto?>>())).ReturnsAsync(stats);

            Services.AddSingleton(mockReviewsService.Object);
            Services.AddSingleton(mockClientService.Object);

            var cut = Render<ReviewStatistics>();
            cut.WaitForState(() => !cut.Markup.Contains("MudProgressCircular"));

            // Act
            var exportBtn = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Export CSV (Detailed)"));
            Assert.NotNull(exportBtn);
            exportBtn.Click();

            // Assert
            JSInterop.VerifyInvoke("eval");
        }
    }
}
