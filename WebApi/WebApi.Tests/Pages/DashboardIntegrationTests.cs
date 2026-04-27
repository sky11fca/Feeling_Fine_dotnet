using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq.Protected;
using MudBlazor.Services;
using WebApi.Models;
using WebApi.Pages;
using WebApi.Services.Business;
using WebApi.Services.Client;
using Xunit;

namespace WebApi.Tests.Pages
{
    public class DashboardIntegrationTests : TestContext
    {
        private readonly Mock<IOptions<ApiSettings>> _optionsMock;

        public DashboardIntegrationTests()
        {
            JSInterop.Mode = JSRuntimeMode.Loose;
            Services.AddMudServices();
            ComponentFactories.AddStub<WebApi.Shared.Navbar>();

            _optionsMock = new Mock<IOptions<ApiSettings>>();
            _optionsMock.Setup(x => x.Value).Returns(new ApiSettings
            {
                ApiUrl = "http://localhost:5160",
                AiUrl = "http://localhost:8000"
            });
        }

        [Fact]
        public void Dashboard_FetchesAndDisplaysData_UsingRealServicesWithMockedHttpMessageHandler()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();

            var businesses = new List<BusinessDto>
            {
                new BusinessDto(Guid.NewGuid(), "Integration Biz", "Integration Industry")
            };
            
            var clients = new List<ClientDto>
            {
                new ClientDto(Guid.NewGuid(), "integration_user", "int@example.com", "9876543210")
            };

            var businessResponse = JsonSerializer.Serialize(businesses);
            var clientResponse = JsonSerializer.Serialize(clients);

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri != null && req.RequestUri.ToString().Contains("/business")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(businessResponse, System.Text.Encoding.UTF8, "application/json")
                });

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri != null && req.RequestUri.ToString().Contains("/client")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(clientResponse, System.Text.Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            
            Services.AddSingleton<IBusinessService>(new BusinessService(httpClient, _optionsMock.Object));
            Services.AddSingleton<IClientService>(new ClientService(httpClient, _optionsMock.Object));

            // Act
            var cut = Render<Dashboard>();

            // Assert - Businesses tab is default
            cut.WaitForState(() => cut.FindAll("tbody tr").Count > 0);
            var businessRows = cut.FindAll("tbody tr");
            Assert.Single(businessRows);
            Assert.Contains("Integration Biz", businessRows[0].InnerHtml);

            // Act - Click clients tab
            var clientsTabBtn = cut.FindAll("button").Single(b => b.TextContent.Trim() == "Clients");
            clientsTabBtn.Click();

            // Assert - Clients tab
            cut.WaitForState(() => cut.FindAll("tbody tr").Count > 0 && cut.FindAll("tbody tr")[0].InnerHtml.Contains("integration_user"));
            var clientRows = cut.FindAll("tbody tr");
            Assert.Single(clientRows);
            Assert.Contains("integration_user", clientRows[0].InnerHtml);
            Assert.Contains("int@example.com", clientRows[0].InnerHtml);
        }
    }
}