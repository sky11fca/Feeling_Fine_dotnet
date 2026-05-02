using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using WebApi.Layout;
using Xunit;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace WebApi.Tests.Layout
{
    public class MainLayoutTests : TestContext, IAsyncLifetime
    {
        public Task InitializeAsync() => Task.CompletedTask;

        async Task IAsyncLifetime.DisposeAsync() => await base.DisposeAsync();

        [Fact]
        public void MainLayout_Renders_Correctly()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            Services.AddMudServices();
            
            // Act
            var cut = Render<MainLayout>(parameters => parameters.Add(p => p.Body, (RenderFragment)(builder => builder.AddContent(0, "<div>Test Content</div>"))));

            // Assert
            Assert.Contains("liquid-glass-content", cut.Markup);
            Assert.Contains("Test Content", cut.Markup);
        }

        [Fact]
        public void MainLayout_Has_MudProviders()
        {
            // Arrange
            JSInterop.Mode = JSRuntimeMode.Loose;
            Services.AddMudServices();

            // Act
            var cut = Render<MainLayout>();

            // Assert
            // Verify that the providers are rendered
            Assert.NotNull(cut.FindComponent<MudBlazor.MudThemeProvider>());
            Assert.NotNull(cut.FindComponent<MudBlazor.MudPopoverProvider>());
            Assert.NotNull(cut.FindComponent<MudBlazor.MudDialogProvider>());
            Assert.NotNull(cut.FindComponent<MudBlazor.MudSnackbarProvider>());
        }
    }
}
