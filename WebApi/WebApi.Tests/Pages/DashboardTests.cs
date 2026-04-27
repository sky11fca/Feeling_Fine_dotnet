using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Services;
using MudBlazor;
using WebApi.Models;
using WebApi.Pages;
using WebApi.Services.Business;
using WebApi.Services.Client;
using WebApi.Shared;

namespace WebApi.Tests.Pages
{
    public class DashboardTests : BunitContext, IAsyncLifetime
    {
        public DashboardTests()
        {
            // Add MudBlazor services required by the component
            Services.AddMudServices();
            
            JSInterop.Mode = JSRuntimeMode.Loose;
            
            // Configure bUnit's JSInterop to mock the localStorage.getItem call
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult("dummy-token");
            
            // Mock the JS call made by MudBlazor's PointerEventsNoneService during disposal
            JSInterop.SetupVoid("mudPointerEventsNone.dispose").SetVoidResult();

            ComponentFactories.AddStub<Navbar>();
        }

        Task IAsyncLifetime.InitializeAsync() => Task.CompletedTask;

        async Task IAsyncLifetime.DisposeAsync()
        {
            // Safely perform asynchronous teardown of the bUnit TestContext / DI container
            if (this is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                Dispose();
            }
        }

        [Fact]
        public void Dashboard_Initialization_LoadsBusinessesByDefault()
        {
            // Arrange
            var mockBizService = new Mock<IBusinessService>();
            var mockClientService = new Mock<IClientService>();
            
            mockBizService.Setup(s => s.GetBusinessQuery(It.IsAny<string>(), It.IsAny<string>()))
                          .ReturnsAsync(new List<BusinessDto?>());

            Services.AddSingleton(mockBizService.Object);
            Services.AddSingleton(mockClientService.Object);

            Render<MudPopoverProvider>();
            Render<MudDialogProvider>();
            var cut = Render<Dashboard>();
            
            // Wait for data to load (progress circular is removed)
            cut.WaitForState(() => !cut.Markup.Contains("MudProgressCircular"));

            // Assert
            mockBizService.Verify(s => s.GetBusinessQuery("", ""), Times.Once);
            Assert.Contains("Businesses", cut.Markup);
            Assert.Contains("Industry", cut.Markup); // Verifies table header is present
        }

        [Fact]
        public void Dashboard_SwitchToClientsTab_LoadsClients()
        {
            // Arrange
            var mockBizService = new Mock<IBusinessService>();
            var mockClientService = new Mock<IClientService>();
            
            mockBizService.Setup(s => s.GetBusinessQuery("", "")).ReturnsAsync(new List<BusinessDto?>());
            mockClientService.Setup(s => s.Query()).ReturnsAsync(new List<ClientDto>());

            Services.AddSingleton(mockBizService.Object);
            Services.AddSingleton(mockClientService.Object);

            Render<MudPopoverProvider>();
            Render<MudDialogProvider>();
            var cut = Render<Dashboard>();
            cut.WaitForState(() => !cut.Markup.Contains("MudProgressCircular"));

            // Act
            var clientsTabBtn = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Clients"));
            Assert.NotNull(clientsTabBtn);
            clientsTabBtn.Click();
            
            // Wait for clients table headers to appear
            cut.WaitForState(() => !cut.Markup.Contains("MudProgressCircular") && cut.Markup.Contains("Username"));

            // Assert
            mockClientService.Verify(s => s.Query(), Times.Once);
            Assert.Contains("Phone Number", cut.Markup);
        }

        [Fact]
        public void Dashboard_CreateNewBusiness_SubmitsSuccessfully()
        {
            // Arrange
            var mockBizService = new Mock<IBusinessService>();
            var mockClientService = new Mock<IClientService>();
            
            mockBizService.Setup(s => s.GetBusinessQuery("", "")).ReturnsAsync(new List<BusinessDto?>());
            mockBizService.Setup(s => s.AddBusiness(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Guid.NewGuid());

            Services.AddSingleton(mockBizService.Object);
            Services.AddSingleton(mockClientService.Object);

            Render<MudPopoverProvider>();
            var dialogProvider = Render<MudDialogProvider>();
            var cut = Render<Dashboard>();
            cut.WaitForState(() => !cut.Markup.Contains("MudProgressCircular"));

            // Act
            var createBtn = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Create New Business"));
            Assert.NotNull(createBtn);
            createBtn.Click();

            // Wait for the dialog to be visible in the provider
            dialogProvider.WaitForState(() => dialogProvider.FindAll("input").Count > 0);

            // Find the mud dialog inputs in the provider
            var inputs = dialogProvider.FindAll("input");
            inputs[0].Change("Awesome Corp");
            inputs[1].Change("Software");

            var submitBtn = dialogProvider.FindAll("button").FirstOrDefault(b => b.TextContent.Trim() == "Create");
            Assert.NotNull(submitBtn);
            submitBtn.Click();

            // Assert
            mockBizService.Verify(s => s.AddBusiness("Awesome Corp", "Software"), Times.Once);
            mockBizService.Verify(s => s.GetBusinessQuery("", ""), Times.Exactly(2)); // Initial load + refresh
        }

        [Fact]
        public void Dashboard_CreateNewClient_SubmitsSuccessfully()
        {
            // Arrange
            var mockBizService = new Mock<IBusinessService>();
            var mockClientService = new Mock<IClientService>();
            
            mockBizService.Setup(s => s.GetBusinessQuery("", "")).ReturnsAsync(new List<BusinessDto?>());
            mockClientService.Setup(s => s.Query()).ReturnsAsync(new List<ClientDto>());
            mockClientService.Setup(s => s.AddAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            Services.AddSingleton(mockBizService.Object);
            Services.AddSingleton(mockClientService.Object);

            Render<MudPopoverProvider>();
            var dialogProvider = Render<MudDialogProvider>();
            var cut = Render<Dashboard>();
            cut.WaitForState(() => !cut.Markup.Contains("MudProgressCircular"));

            // Switch to Clients tab
            cut.FindAll("button").First(b => b.TextContent.Contains("Clients")).Click();
            cut.WaitForState(() => !cut.Markup.Contains("MudProgressCircular") && cut.Markup.Contains("Username"));

            // Act
            cut.FindAll("button").First(b => b.TextContent.Contains("Create New Client")).Click();

            // Wait for the dialog to be visible in the provider
            dialogProvider.WaitForState(() => dialogProvider.FindAll("input").Count > 0);

            var inputs = dialogProvider.FindAll("input");
            inputs[0].Change("johndoe");
            inputs[1].Change("john@example.com");
            inputs[2].Change("555-1234");

            var submitBtn = dialogProvider.FindAll("button").FirstOrDefault(b => b.TextContent.Trim() == "Create");
            Assert.NotNull(submitBtn);
            submitBtn.Click();

            // Assert
            mockClientService.Verify(s => s.AddAsync("johndoe", "john@example.com", "555-1234"), Times.Once);
            mockClientService.Verify(s => s.Query(), Times.Exactly(2)); // Initial switch + refresh
        }
        
        [Fact]
        public void Dashboard_CreateNewBusiness_EmptyName_DoesNotSubmit()
        {
            // Arrange
            var mockBizService = new Mock<IBusinessService>();
            var mockClientService = new Mock<IClientService>();
            
            mockBizService.Setup(s => s.GetBusinessQuery("", "")).ReturnsAsync(new List<BusinessDto?>());

            Services.AddSingleton(mockBizService.Object);
            Services.AddSingleton(mockClientService.Object);

            Render<MudPopoverProvider>();
            var dialogProvider = Render<MudDialogProvider>();
            var cut = Render<Dashboard>();
            cut.WaitForState(() => !cut.Markup.Contains("MudProgressCircular"));

            // Act
            cut.FindAll("button").First(b => b.TextContent.Contains("Create New Business")).Click();

            // Wait for the dialog to appear
            dialogProvider.WaitForAssertion(() => Assert.NotNull(dialogProvider.FindAll("button").FirstOrDefault(b => b.TextContent.Trim() == "Create")));

            // Leave inputs empty, just click Create
            dialogProvider.FindAll("button").First(b => b.TextContent.Trim() == "Create").Click();

            // Assert
            mockBizService.Verify(s => s.AddBusiness(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockBizService.Verify(s => s.GetBusinessQuery("", ""), Times.Once); // Only the initial load should have happened
        }
    }
}