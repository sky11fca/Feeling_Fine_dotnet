using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Businesses.Queries;
using DotnetApi.Domains.Entities;
using FluentAssertions;
using Moq;

namespace DotnetApi.Tests.Application.Businesses.Query;

public class GetBusinessQueryTests
{
    private readonly Mock<IBusinessRepository> _businessRepositoryMock;
    private readonly GetBusinessQueryHandler _handler;

    public GetBusinessQueryTests()
    {
        _businessRepositoryMock = new Mock<IBusinessRepository>();
        _handler = new GetBusinessQueryHandler(_businessRepositoryMock.Object);
    }

    [Fact]
    public async Task GivenGetAllBusinessesQueryRequestWhenCalledThenReturnsBusinesses()
    {
        //Arrange
        
        var business = Business.Create("Test Business", "Technology");
        var businesses = new List<Business> {business}.AsQueryable();
        _businessRepositoryMock.Setup(r => r.Query()).Returns(businesses);
        var query = new GetBusinessQuery(string.Empty, string.Empty);


        //Act

        var result = await _handler.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Test Business");
    }
}