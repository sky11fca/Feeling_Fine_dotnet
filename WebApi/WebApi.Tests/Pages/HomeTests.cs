using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using WebApi.Models;
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
            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto>());

            Services.AddSingleton(mockBusinessService.Object);

            // Act
            var cut = Render<Home>();

            // Assert
            cut.Find("h3").MarkupMatches("<h3>Welcome</h3>");
            cut.Find("p").MarkupMatches("<p>Choose a Company</p>");
        }

        [Fact]
        public void Renders_BusinessList_WhenBusinessesExist()
        {
            // Arrange
            var businesses = new List<BusinessDto>
            {
                new BusinessDto(Guid.NewGuid(), "Biz 1", "Tech" ),
                new BusinessDto (Guid.NewGuid(), "Biz 2", "Food")
            };

            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(string.Empty, string.Empty))
                .ReturnsAsync(businesses);

            Services.AddSingleton(mockBusinessService.Object);

            // Act
            var cut = Render<Home>();

            // Assert
            var listItems = cut.FindAll("li");
            Assert.Equal(2, listItems.Count);
            Assert.Contains("Biz 1", listItems[0].TextContent);
            Assert.Contains("Biz 2", listItems[1].TextContent);
        }

        [Fact]
        public void Renders_NoBusinessesFound_WhenListIsEmpty()
        {
            // Arrange
            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto>());

            Services.AddSingleton(mockBusinessService.Object);

            // Act
            var cut = Render<Home>();

            // Assert
            // Index 1 because index 0 is "Choose a Company"
            cut.FindAll("p")[1].MarkupMatches("<p>No businesses found</p>");
        }

        [Fact]
        public void Renders_NoBusinessesFound_WhenServiceThrows()
        {
            // Arrange
            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Service failure"));

            Services.AddSingleton(mockBusinessService.Object);

            // Act
            var cut = Render<Home>();

            // Assert
            cut.FindAll("p")[1].MarkupMatches("<p>No businesses found</p>");
        }

        [Fact]
        public void Navigates_ToReviews_WhenBusinessClicked()
        {
            // Arrange
            var businessId = Guid.NewGuid();
            var businesses = new List<BusinessDto>
            {
                new BusinessDto(businessId, "Biz Test", "Test" ),
            };

            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(string.Empty, string.Empty))
                .ReturnsAsync(businesses);

            Services.AddSingleton(mockBusinessService.Object);

            // Act
            var cut = Render<Home>();
            cut.Find("li").Click();

            // Assert
            var navMan = Services.GetRequiredService<NavigationManager>();
            Assert.EndsWith($"/reviews/{businessId}", navMan.Uri);
        }
    }
}