using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using MudBlazor.Services;
using WebApi.Models;
using WebApi.Pages;
using WebApi.Services.Business;
using WebApi.Services.Client;
using WebApi.Services.Reply;
using WebApi.Services.Reviews;
using WebApi.Shared;
using Xunit;

namespace WebApi.Tests.Pages
{
    public class ReviewsTests : TestContext, IAsyncLifetime
    {
        public ReviewsTests()
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
            
            var mockBusinessService = new Mock<IBusinessService>();
            var mockReviewsService = new Mock<IReviewsService>();
            var mockClientService = new Mock<IClientService>();
            var mockReplyService = new Mock<IReplyService>();

            Services.AddSingleton(mockBusinessService.Object);
            Services.AddSingleton(mockReviewsService.Object);
            Services.AddSingleton(mockClientService.Object);
            Services.AddSingleton(mockReplyService.Object);

            var navMan = Services.GetRequiredService<NavigationManager>();

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Reviews>(parameters => parameters.Add(p => p.BusinessId, Guid.NewGuid()));

            // Assert
            Assert.EndsWith("/login", navMan.Uri);
        }

        [Fact]
        public void Renders_BusinessDetails_WhenBusinessExists()
        {
            // Arrange
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult("token");
            var businessId = Guid.NewGuid();
            var business = new BusinessDto(businessId, "Test Biz", "Test Industry");

            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto> { business });

            var mockReviewsService = new Mock<IReviewsService>();
            mockReviewsService.Setup(s => s.GetReviewQuery(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<ReviewDto?>());

            var mockClientService = new Mock<IClientService>();
            mockClientService.Setup(s => s.Query()).ReturnsAsync(new List<ClientDto>());

            var mockReplyService = new Mock<IReplyService>();

            Services.AddSingleton(mockBusinessService.Object);
            Services.AddSingleton(mockReviewsService.Object);
            Services.AddSingleton(mockClientService.Object);
            Services.AddSingleton(mockReplyService.Object);

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Reviews>(parameters => parameters
                .Add(p => p.BusinessId, businessId));

            // Assert
            Assert.Contains($"Reviews for {business.Name}", cut.Markup);
            Assert.Contains($"{business.Industry}", cut.Markup);
        }

        [Fact]
        public void Renders_GenericHeader_WhenBusinessNotFound()
        {
            // Arrange
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult("token");
            var businessId = Guid.NewGuid();

            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto>());

            var mockReviewsService = new Mock<IReviewsService>();
            mockReviewsService.Setup(s => s.GetReviewQuery(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<ReviewDto?>());

            var mockClientService = new Mock<IClientService>();
            mockClientService.Setup(s => s.Query()).ReturnsAsync(new List<ClientDto>());

            var mockReplyService = new Mock<IReplyService>();

            Services.AddSingleton(mockBusinessService.Object);
            Services.AddSingleton(mockReviewsService.Object);
            Services.AddSingleton(mockClientService.Object);
            Services.AddSingleton(mockReplyService.Object);

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Reviews>(parameters => parameters
                .Add(p => p.BusinessId, businessId));

            // Assert
            Assert.Contains("Reviews", cut.Find("h1").TextContent);
        }

        [Fact]
        public void Renders_ReviewsList_WhenReviewsExist()
        {
            // Arrange
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult("token");
            var businessId = Guid.NewGuid();
            var clientId1 = Guid.NewGuid();
            var clientId2 = Guid.NewGuid();
            var reviews = new List<ReviewDto?>
            {
                new ReviewDto ( Guid.NewGuid(), clientId1, 5.0m, "Great!",  "TEXT1",  "test", "TEST", 0.999 ),
                new ReviewDto ( Guid.NewGuid(), clientId2, 1.0m, "Bad!",  "TEXT2",  "test", "TEST", 0.999 ),
            };

            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto>());

            var mockReviewsService = new Mock<IReviewsService>();
            mockReviewsService.Setup(s => s.GetReviewQuery(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(reviews);

            var mockClientService = new Mock<IClientService>();
            mockClientService.Setup(s => s.Query()).ReturnsAsync(new List<ClientDto>());
            mockClientService.Setup(s => s.FindAsync(clientId1)).ReturnsAsync(new ClientDto(clientId1, "User1", "e1", "123"));
            mockClientService.Setup(s => s.FindAsync(clientId2)).ReturnsAsync(new ClientDto(clientId2, "User2", "e2", "123"));

            var mockReplyService = new Mock<IReplyService>();
            mockReplyService.Setup(s => s.GetRepliesAsync(It.IsAny<Guid>())).ReturnsAsync(new List<ReplyDto>());

            Services.AddSingleton(mockBusinessService.Object);
            Services.AddSingleton(mockReviewsService.Object);
            Services.AddSingleton(mockClientService.Object);
            Services.AddSingleton(mockReplyService.Object);

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Reviews>(parameters => parameters
                .Add(p => p.BusinessId, businessId));

            // Assert
            Assert.Contains("Great!", cut.Markup);
            Assert.Contains("Bad!", cut.Markup);
            Assert.Contains("User1", cut.Markup);
            Assert.Contains("User2", cut.Markup);
        }

        [Fact]
        public void Submits_Review_Successfully()
        {
            // Arrange
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult("token");
            var businessId = Guid.NewGuid();
            
            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto>());

            var mockReviewsService = new Mock<IReviewsService>();
            mockReviewsService.Setup(s => s.GetReviewQuery(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<ReviewDto?>());

            var mockClientService = new Mock<IClientService>();
            mockClientService.Setup(s => s.Query()).ReturnsAsync(new List<ClientDto>());

            var mockReplyService = new Mock<IReplyService>();
            mockReplyService.Setup(s => s.GetRepliesAsync(It.IsAny<Guid>())).ReturnsAsync(new List<ReplyDto>());

            Services.AddSingleton(mockBusinessService.Object);
            Services.AddSingleton(mockReviewsService.Object);
            Services.AddSingleton(mockClientService.Object);
            Services.AddSingleton(mockReplyService.Object);

            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Reviews>(parameters => parameters
                .Add(p => p.BusinessId, businessId));

            // Act
            var numberInput = cut.Find("input[type='number']");
            numberInput.Change("4"); // pass as string for input change

            var textInputs = cut.FindAll("textarea");
            if (textInputs.Count > 0)
            {
                textInputs[0].Change("Good service");
            }
            
            cut.Find("button[type='submit']").Click();

            // Assert
            mockReviewsService.Verify(s => s.AddReview(It.IsAny<Guid>(), It.IsAny<Guid>(), 4, "Good service", "FeelingFine.net"), Times.Once);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        Task IAsyncLifetime.DisposeAsync()
        {
            if (this is System.IAsyncDisposable asyncDisposable)
            {
                return asyncDisposable.DisposeAsync().AsTask();
            }
            return Task.CompletedTask;
        }
    }
}