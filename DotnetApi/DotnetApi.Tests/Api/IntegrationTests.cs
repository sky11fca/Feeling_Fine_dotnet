using DotnetApi.Application.Authentication.Command;
using DotnetApi.Application.Clients.Commands;
using DotnetApi.Application.Clients.Query;
using DotnetApi.Application.Reply.Command;
using DotnetApi.Application.Reply.Query;
using DotnetApi.Application.Reviews.Commands;
using DotnetApi.Application.Reviews.Queries;
using DotnetApi.Application.User.Query;
using DotnetApi.Domains.Entities;
using DotnetApi.Domains.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace DotnetApi.Tests.Api;

public class IntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public IntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task RegisterAndLogin_ReturnsToken()
    {
        // Arrange
        var registerCommand = new RegisterCommand( "testuser", Guid.NewGuid(),"test1@example.com", "Password123!", "Admin");
        var loginCommand = new LoginCommand("test1@example.com", "Password123!");

        // Act - Register
        var registerResponse = await _client.PostAsJsonAsync("/api/authentication/register", registerCommand);
        registerResponse.EnsureSuccessStatusCode();

        // Act - Login
        var loginResponse = await _client.PostAsJsonAsync("/api/authentication/login", loginCommand);
        loginResponse.EnsureSuccessStatusCode();

        var token = await loginResponse.Content.ReadAsStringAsync();

        // Assert
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Clients_AddAndRetrieve_ReturnsClient()
    {
        // Arrange
        var addClientCommand = new AddClientCommand("client1", "client1@example.com", "+12223334444");

        // Act - Add
        var addResponse = await _client.PostAsJsonAsync("/api/v1/client", addClientCommand);
        addResponse.EnsureSuccessStatusCode();
        var createdClient = await addResponse.Content.ReadFromJsonAsync<ClientDto>();
        var clientId = createdClient!.Id;

        // Act - Retrieve All
        var getAllResponse = await _client.GetAsync("/api/v1/client");
        getAllResponse.EnsureSuccessStatusCode();
        var clients = await getAllResponse.Content.ReadFromJsonAsync<List<ClientDto>>();

        // Assert
        clientId.Should().NotBeEmpty();
        clients.Should().NotBeNull();
        clients.Should().Contain(c => c.Id == clientId && c.Username == "client1");
        
        // Act - Retrieve By Id
        var getByIdResponse = await _client.GetAsync($"/api/v1/client/{clientId}");
        getByIdResponse.EnsureSuccessStatusCode();
        var client = await getByIdResponse.Content.ReadFromJsonAsync<ClientDto>();
        
        // Assert
        client.Should().NotBeNull();
        client!.Id.Should().Be(clientId);
        client.Username.Should().Be("client1");
    }

    [Fact]
    public async Task ReviewsAndReplies_AddAndRetrieve_ReturnsData()
    {
        // Arrange
        var businessId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var addReviewCommand = new AddReviewCommand(businessId, clientId, 4.5m, "Great!", "2023-10-27");

        // Act - Add Review
        var addReviewResponse = await _client.PostAsJsonAsync("/api/v1/review", addReviewCommand);
        addReviewResponse.EnsureSuccessStatusCode();
        var createdReview = await addReviewResponse.Content.ReadFromJsonAsync<ReviewDto>();
        var reviewId = createdReview!.Id;

        // Act - Get Review
        var getReviewResponse = await _client.GetAsync($"/api/v1/review?BusinessId={businessId}");
        getReviewResponse.EnsureSuccessStatusCode();
        var reviews = await getReviewResponse.Content.ReadFromJsonAsync<List<ReviewDto>>();

        // Assert Review
        reviewId.Should().NotBeEmpty();
        reviews.Should().NotBeNull();
        reviews.Should().Contain(r => r.Id == reviewId && r.RawText == "Great!");

        // Arrange - Add Reply
        var toClientId = Guid.NewGuid();
        var addReplyCommand = new AddReplyCommand(reviewId, toClientId, "Thanks!");

        // Act - Add Reply
        var addReplyResponse = await _client.PostAsJsonAsync("/api/v1/reply", addReplyCommand);
        addReplyResponse.EnsureSuccessStatusCode();

        // Act - Get Replies
        var getRepliesResponse = await _client.GetAsync($"/api/v1/reply?ReviewId={reviewId}");
        getRepliesResponse.EnsureSuccessStatusCode();
        var replies = await getRepliesResponse.Content.ReadFromJsonAsync<List<ReplyDto>>();

        // Assert Reply
        replies.Should().NotBeNull();
        replies.Should().Contain(r => r.ReviewId == reviewId && r.RawText == "Thanks!");
    }
}