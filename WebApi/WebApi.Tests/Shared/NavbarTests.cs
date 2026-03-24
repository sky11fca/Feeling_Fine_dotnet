using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MudBlazor.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using WebApi.Shared;
using Xunit;

namespace WebApi.Tests.Shared
{
    public class NavbarTests : TestContext, IAsyncLifetime
    {
        [Fact]
        public void Renders_Without_Errors_When_Token_Missing()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult(string.Empty);
            Services.AddMudServices();

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Navbar>();

            // Assert
            Assert.Contains("FeelingFine", cut.Markup);
        }

        [Fact]
        public void Parses_And_Displays_Email_From_Token()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            
            // Create a fake JWT token with an email claim
            var payloadJson = "{\"email\":\"testuser@example.com\"}";
            var payloadBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payloadJson)).TrimEnd('=');
            var fakeToken = $"header.{payloadBase64}.signature";

            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult(fakeToken);
            Services.AddMudServices();

            // Act
            Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Navbar>();

            // Assert
            Assert.Contains("FeelingFine", cut.Markup);
            Assert.Contains("testuser@example.com", cut.Markup);
        }

        [Fact]
        public async Task Logout_Removes_Tokens_And_Redirects_To_Home()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            JSInterop.Setup<string>("localStorage.getItem", "authToken").SetResult(string.Empty);
            
            var removeAuthToken = JSInterop.SetupVoid("localStorage.removeItem", "authToken").SetVoidResult();
            var removeBusinessId = JSInterop.SetupVoid("localStorage.removeItem", "businessId").SetVoidResult();
            
            Services.AddMudServices();
            var navMan = Services.GetRequiredService<NavigationManager>();

            var popoverProvider = Render<MudBlazor.MudPopoverProvider>();
            var cut = Render<Navbar>();

            // Act
            // Open the MudMenu to render its inner items
            cut.Find("button").Click();

            popoverProvider.WaitForState(() => popoverProvider.FindComponents<MudBlazor.MudMenuItem>().Count > 0);
            var menuItem = popoverProvider.FindComponent<MudBlazor.MudMenuItem>();
            await menuItem.InvokeAsync(() => menuItem.Instance.OnClick.InvokeAsync());

            // Assert
            Assert.Equal(1, removeAuthToken.Invocations.Count);
            Assert.Equal(1, removeBusinessId.Invocations.Count);
            Assert.EndsWith("/", navMan.Uri);
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