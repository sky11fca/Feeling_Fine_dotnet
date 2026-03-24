using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Pages;
using MudBlazor.Services;
using WebApi.Services.Authentication;
using Xunit;
using System;
using Microsoft.AspNetCore.Components;

namespace WebApi.Tests.Pages
{
    public class LoginTests : TestContext, IAsyncLifetime
    {
        [Fact]
        public void Renders_LoginComponent()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            JSInterop.Setup<string>("localStorage.getItem", _ => true).SetResult(string.Empty);
            var mockAuthService = new Mock<IAuthenticationService>();
            
            Services.AddSingleton(mockAuthService.Object);
            Services.AddMudServices();

            // Act
            var cut = Render<Login>();

            // Assert
            cut.Find("h1").MarkupMatches("<h1 class=\"mud-typography mud-typography-h1 mud-typography-align-center mb-16\">Login</h1>");
        }

        [Fact]
        public void Redirects_ToReviews_When_Token_And_BusinessId_Exist()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            var businessId = Guid.NewGuid().ToString();
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult("dummyToken");
            JSInterop.Setup<string>("localStorage.getItem", "businessId").SetResult(businessId);
            
            var mockAuthService = new Mock<IAuthenticationService>();
            Services.AddSingleton(mockAuthService.Object);
            Services.AddMudServices();

            var navMan = Services.GetRequiredService<NavigationManager>();

            // Act
            var cut = Render<Login>();

            // Assert
            Assert.EndsWith($"/reviews/{businessId}", navMan.Uri);
        }

        [Fact]
        public void Performs_Login_And_Redirects_Successfully()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            JSInterop.Setup<string>("localStorage.getItem", _ => true).SetResult(string.Empty);
            var mockAuthService = new Mock<IAuthenticationService>();
            var businessId = Guid.NewGuid().ToString();
            
            var payloadJson = $"{{\"jti\":\"{businessId}\"}}";
            var payloadBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payloadJson)).TrimEnd('=');
            var fakeToken = $"header.{payloadBase64}.signature";

            mockAuthService.Setup(s => s.Login("test@test.com", "password"))
                .ReturnsAsync(fakeToken);
            
            Services.AddSingleton(mockAuthService.Object);
            Services.AddMudServices();

            var navMan = Services.GetRequiredService<NavigationManager>();

            var cut = Render<Login>();

            // Act
            var emailInput = cut.Find("input[type='email']");
            emailInput.Change("test@test.com");
            
            var passwordInput = cut.Find("input[type='password']");
            passwordInput.Change("password");

            cut.Find("button").Click();

            // Assert
            mockAuthService.Verify(s => s.Login("test@test.com", "password"), Times.Once);
            Assert.EndsWith($"/reviews/{businessId}", navMan.Uri);
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