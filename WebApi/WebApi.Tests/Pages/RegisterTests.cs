using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MudBlazor.Services;
using WebApi.Models;
using WebApi.Models.Requests;
using WebApi.Pages;
using WebApi.Services.Authentication;
using WebApi.Services.Business;
using Xunit;
using Microsoft.AspNetCore.Components;

namespace WebApi.Tests.Pages
{
    public class RegisterTests : TestContext, IAsyncLifetime
    {
        [Fact]
        public void Renders_RegisterComponent()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            JSInterop.Setup<string>("localStorage.getItem", _ => true).SetResult(string.Empty);
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockBusinessService = new Mock<IBusinessService>();
            
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto>());

            Services.AddSingleton(mockAuthService.Object);
            Services.AddSingleton(mockBusinessService.Object);
            Services.AddMudServices();

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Register>();

            // Assert
            cut.Find("h1").MarkupMatches("<h1 class=\"mud-typography mud-typography-h1 mud-typography-align-center mb-16\">Register</h1>");
        }

        [Fact]
        public void Redirects_ToReviews_When_Token_And_BusinessId_Exist()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            var businessId = Guid.NewGuid().ToString();
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult("token");
            JSInterop.Setup<string>("localStorage.getItem", "businessId").SetResult(businessId);
            
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto>());

            Services.AddSingleton(mockAuthService.Object);
            Services.AddSingleton(mockBusinessService.Object);
            Services.AddMudServices();

            var navMan = Services.GetRequiredService<NavigationManager>();

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Register>();

            // Assert
            Assert.EndsWith($"/reviews/{businessId}", navMan.Uri);
        }

        [Fact]
        public void Shows_Error_When_Passwords_Do_Not_Match()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            JSInterop.Setup<string>("localStorage.getItem", _ => true).SetResult(string.Empty);
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockBusinessService = new Mock<IBusinessService>();
            
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto>());

            Services.AddSingleton(mockAuthService.Object);
            Services.AddSingleton(mockBusinessService.Object);
            Services.AddMudServices();

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Register>();

            var passwordInputs = cut.FindAll("input[type='password']");
            passwordInputs[0].Change("pass1");
            passwordInputs[1].Change("pass2");

            cut.Find("button").Click();

            // Assert
            Assert.Contains("Passwords do not match.", cut.Markup);
            mockAuthService.Verify(s => s.Register(It.IsAny<RegisterCommand>()), Times.Never);
        }

        [Fact]
        public void Registers_Successfully_And_Redirects_To_Login()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            JSInterop.Setup<string>("localStorage.getItem", _ => true).SetResult(string.Empty);
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockBusinessService = new Mock<IBusinessService>();
            
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto>());

            mockAuthService.Setup(s => s.Register(It.IsAny<RegisterCommand>())).ReturnsAsync(default(UserDto)!);

            Services.AddSingleton(mockAuthService.Object);
            Services.AddSingleton(mockBusinessService.Object);
            Services.AddMudServices();
            
            var navMan = Services.GetRequiredService<NavigationManager>();

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Register>();

            var textInputs = cut.FindAll("input[type='text']");
            if (textInputs.Count > 0) textInputs[0].Change("testuser");
            
            var emailInput = cut.Find("input[type='email']");
            emailInput.Change("test@test.com");

            var passwordInputs = cut.FindAll("input[type='password']");
            passwordInputs[0].Change("password");
            passwordInputs[1].Change("password");

            cut.Find("button").Click();

            // Assert
            mockAuthService.Verify(s => s.Register(It.IsAny<RegisterCommand>()), Times.Once);
            Assert.EndsWith("/login", navMan.Uri);
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