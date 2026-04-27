using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using WebApi.Pages;
using WebApi.Services.Business;
using Xunit;

namespace WebApi.Tests.Pages
{
    public class HomeTests : TestContext
    {
        [Fact]
        public void Renders_WelcomeMessage()
        {
            // Arrange
            JSInterop.Setup<string>("localStorage.getItem", _ => true).SetResult(string.Empty);
            var mockBusinessService = new Mock<IBusinessService>();
            Services.AddSingleton(mockBusinessService.Object);

            // Act
            var cut = Render<Home>();

            // Assert
            Assert.Contains("FeelingFine", cut.Markup);
            Assert.Contains("Empower your workplace culture.", cut.Markup);
        }

        [Fact]
        public void Redirects_ToReviews_WhenTokenAndBusinessIdExist()
        {
            // Arrange
            var businessId = Guid.NewGuid().ToString();
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult("token");
            JSInterop.Setup<string>("localStorage.getItem", "businessId").SetResult(businessId);
            
            var mockBusinessService = new Mock<IBusinessService>();
            Services.AddSingleton(mockBusinessService.Object);
            
            var navMan = Services.GetRequiredService<NavigationManager>();

            // Act
            var cut = Render<Home>();

            // Assert
            Assert.EndsWith("/reviews", navMan.Uri);
        }

        [Fact]
        public void Navigates_ToLogin_WhenLoginClicked()
        {
            // Arrange
            JSInterop.Setup<string>("localStorage.getItem", _ => true).SetResult(string.Empty);
            var mockBusinessService = new Mock<IBusinessService>();
            Services.AddSingleton(mockBusinessService.Object);
            
            var navMan = Services.GetRequiredService<NavigationManager>();

            var cut = Render<Home>();

            // Act
            var buttons = cut.FindAll("button");
            var loginButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Login"));
            loginButton?.Click();

            // Assert
            Assert.EndsWith("/login", navMan.Uri);
        }

        [Fact]
        public void Navigates_ToRegister_WhenRegisterClicked()
        {
            // Arrange
            JSInterop.Setup<string>("localStorage.getItem", _ => true).SetResult(string.Empty);
            var mockBusinessService = new Mock<IBusinessService>();
            Services.AddSingleton(mockBusinessService.Object);
            
            var navMan = Services.GetRequiredService<NavigationManager>();

            var cut = Render<Home>();

            // Act
            var buttons = cut.FindAll("button");
            var registerButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Register"));
            registerButton?.Click();

            // Assert
            Assert.EndsWith("/register", navMan.Uri);
        }
    }
}