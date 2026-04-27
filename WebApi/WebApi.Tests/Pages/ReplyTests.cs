using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using WebApi.Pages;
using WebApi.Services.Reply;
using WebApi.Shared;
using Xunit;

namespace WebApi.Tests.Pages
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseContent;
        public MockHttpMessageHandler(string responseContent)
        {
            _responseContent = responseContent;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(_responseContent)
            });
        }
    }

    public class ReplyTests : TestContext, IAsyncLifetime
    {
        public ReplyTests()
        {
            ComponentFactories.AddStub<Navbar>();
        }

        [Fact]
        public void Redirects_To_Login_When_Token_Missing()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            JSInterop.Setup<string>("localStorage.getItem", _ => true).SetResult(string.Empty);
            var mockReplyService = new Mock<IReplyService>();
            
            Services.AddSingleton(mockReplyService.Object);
            Services.AddMudServices();
            Services.AddSingleton(new HttpClient(new MockHttpMessageHandler("{\"reply\":\"\"}")) { BaseAddress = new Uri("http://localhost/") });
            
            var navMan = Services.GetRequiredService<NavigationManager>();

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Reply>();

            // Assert
            Assert.EndsWith("/login", navMan.Uri);
        }

        [Fact]
        public void Initializes_And_Fetches_Ai_Reply()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            JSInterop.Setup<string>("localStorage.getItem", _ => true).SetResult("token");
            var mockReplyService = new Mock<IReplyService>();
            
            Services.AddSingleton(mockReplyService.Object);
            Services.AddMudServices();
            
            var aiReplyJson = "{\"reply\":\"Thank you for the review!\"}";
            var httpClient = new HttpClient(new MockHttpMessageHandler(aiReplyJson)) { BaseAddress = new Uri("http://localhost/") };
            Services.AddSingleton(httpClient);

            var navMan = Services.GetRequiredService<NavigationManager>();
            var uri = navMan.GetUriWithQueryParameters(new Dictionary<string, object?>
            {
                { "ClientName", "John Doe" },
                { "RawText", "Great place" },
                { "Sentiment", "Positive" }
            });
            navMan.NavigateTo(uri);

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Reply>(parameters => parameters
                .Add(p => p.ReviewId, Guid.NewGuid()));

            // Assert
            cut.WaitForState(() => !cut.Markup.Contains("Generating AI reply..."), TimeSpan.FromSeconds(2));
            Assert.Contains("Thank you for the review!", cut.Markup);
            Assert.Contains("Review by John Doe", cut.Markup);
        }

        [Fact]
        public void Submits_Reply_Successfully()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            JSInterop.Setup<string>("localStorage.getItem", _ => true).SetResult("token");
            var mockReplyService = new Mock<IReplyService>();
            mockReplyService.Setup(s => s.AddReviewAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            
            Services.AddSingleton(mockReplyService.Object);
            Services.AddMudServices();
            
            var httpClient = new HttpClient(new MockHttpMessageHandler("{\"reply\":\"Automated text\"}")) { BaseAddress = new Uri("http://localhost/") };
            Services.AddSingleton(httpClient);

            var navMan = Services.GetRequiredService<NavigationManager>();
            var businessId = Guid.NewGuid();
            var uri = navMan.GetUriWithQueryParameters(new Dictionary<string, object?>
            {
                { "BusinessId", businessId },
                { "ClientId", Guid.NewGuid() }
            });
            navMan.NavigateTo(uri);

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Reply>(parameters => parameters
                .Add(p => p.ReviewId, Guid.NewGuid()));

            cut.WaitForState(() => !cut.Markup.Contains("Generating AI reply..."));

            var textInputs = cut.FindAll("textarea");
            if (textInputs.Count > 0)
            {
                textInputs[0].Change("Manual override text");
            }

            cut.Find("button[type='submit']").Click();

            // Assert
            mockReplyService.Verify(s => s.AddReviewAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), "Manual override text"), Times.Once);
            Assert.EndsWith("/reviews", navMan.Uri);
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