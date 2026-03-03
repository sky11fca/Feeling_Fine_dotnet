using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using WebApi.Models;
using WebApi.Pages;
using WebApi.Services.Business;
using WebApi.Services.Reviews;
using Xunit;

namespace WebApi.Tests.Pages
{
    public class ReviewsTests : TestContext
    {
        [Fact]
        public void Renders_BusinessDetails_WhenBusinessExists()
        {
            // Arrange
            var businessId = Guid.NewGuid();
            var business = new BusinessDto(businessId, "Test Biz", "Test Industry");

            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto> { business });

            var mockReviewsService = new Mock<IReviewsService>();
            mockReviewsService.Setup(s => s.GetReviewQuery(businessId, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<ReviewDto?>());

            Services.AddSingleton(mockBusinessService.Object);
            Services.AddSingleton(mockReviewsService.Object);

            // Act
            var cut = Render<Reviews>(parameters => parameters
                .Add(p => p.BusinessId, businessId));

            // Assert
            cut.Find("h3").MarkupMatches($"<h3>Reviews for {business.Name}</h3>");
            cut.Find("p").MarkupMatches($"<p>{business.Industry}</p>");
        }

        [Fact]
        public void Renders_GenericHeader_WhenBusinessNotFound()
        {
            // Arrange
            var businessId = Guid.NewGuid();

            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto>());

            var mockReviewsService = new Mock<IReviewsService>();
            mockReviewsService.Setup(s => s.GetReviewQuery(businessId, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<ReviewDto?>());

            Services.AddSingleton(mockBusinessService.Object);
            Services.AddSingleton(mockReviewsService.Object);

            // Act
            var cut = Render<Reviews>(parameters => parameters
                .Add(p => p.BusinessId, businessId));

            // Assert
            cut.Find("h3").MarkupMatches("<h3>Reviews</h3>");
        }

        [Fact]
        public void Renders_ReviewsList_WhenReviewsExist()
        {
            // Arrange
            var businessId = Guid.NewGuid();
            var reviews = new List<ReviewDto?>
            {
                new ReviewDto ( Guid.NewGuid(), 5.0m, "Great!",  "TEXT1",  "test", "TEST", 0.999 ),
                new ReviewDto ( Guid.NewGuid(), 1.0m, "Bad!",  "TEXT2",  "test", "TEST", 0.999 ),
            };

            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto>());

            var mockReviewsService = new Mock<IReviewsService>();
            mockReviewsService.Setup(s => s.GetReviewQuery(businessId, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(reviews);

            Services.AddSingleton(mockBusinessService.Object);
            Services.AddSingleton(mockReviewsService.Object);

            // Act
            var cut = Render<Reviews>(parameters => parameters
                .Add(p => p.BusinessId, businessId));

            // Assert
            var listItems = cut.FindAll("li");
            Assert.Equal(2, listItems.Count);
            Assert.Contains("Great!", listItems[0].TextContent);
            Assert.Contains("Bad!", listItems[1].TextContent);
        }

        [Fact]
        public void Submits_Review_Successfully()
        {
            // Arrange
            var businessId = Guid.NewGuid();
            var mockBusinessService = new Mock<IBusinessService>();
            mockBusinessService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<BusinessDto>());

            var mockReviewsService = new Mock<IReviewsService>();
            mockReviewsService.Setup(s => s.GetReviewQuery(businessId, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<ReviewDto?>());

            Services.AddSingleton(mockBusinessService.Object);
            Services.AddSingleton(mockReviewsService.Object);

            var cut = Render<Reviews>(parameters => parameters
                .Add(p => p.BusinessId, businessId));

            // Act
            cut.Find("input[type='number']").Change(4);
            cut.Find("textarea").Change("Good service");
            cut.Find("button[type='submit']").Click();

            // Assert
            mockReviewsService.Verify(s => s.AddReview(businessId, 4, "Good service", "FeelingFine"), Times.Once);
            mockReviewsService.Verify(s => s.GetReviewQuery(businessId, It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }
    }
}