using DotnetApi.Application.Reviews.Commands;
using DotnetApi.Application.Reviews.Validators;
using FluentAssertions;

namespace DotnetApi.Tests.Validators;

public class AddReviewValidatorTests : IDisposable
{
    private AddReviewValidator _sut;
    public AddReviewValidatorTests()
    {
        _sut = CreateSut();
    }

    [Fact]
    public void GivenEmptyBusinessIdWhenValidatingThenReturnsError()
    {
        var review = new AddReviewCommand(Guid.Empty, 1.5m,"testing", "test");
        
        var result = _sut.Validate(review);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
    }


    [Fact]
    public void GivenInvalidRawTestWhenValidatingThenReturnsError()
    {
        var review = new AddReviewCommand(Guid.NewGuid(), 1.5m,string.Empty, "test");
        
        var result = _sut.Validate(review);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
    }

    [Fact]
    public void GivenInvalidSubmitedOnWhenValidatingThenReturnsError()
    {
        var review = new AddReviewCommand(Guid.NewGuid(), 1.5m,"testing", string.Empty);
        
        var result = _sut.Validate(review);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
    }

    [Fact]
    public void GivenOutOfRangeReviewWhenValidatingThenReturnsError()
    {
        var review = new AddReviewCommand(Guid.NewGuid(), 6.9m,"testing", string.Empty);
        
        var result = _sut.Validate(review);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void GivenValidParametersWhenValidatingTHenReturnsNoErrors()
    {
        var review = new AddReviewCommand(Guid.NewGuid(), 1.5m,"testing", "test");
        
        var result = _sut.Validate(review);
        
        result.IsValid.Should().BeTrue();
    }

    private AddReviewValidator CreateSut() => new();
    
    public void Dispose()
    {
        
    }
}