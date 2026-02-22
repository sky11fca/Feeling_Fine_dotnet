using DotnetApi.Application.Businesses.Commands;
using DotnetApi.Application.Businesses.Validators;
using FluentAssertions;

namespace DotnetApi.Tests.Validators;

public class AddBusinessValidatorTests : IDisposable
{
    private AddBusinessValidator _sut;
    public AddBusinessValidatorTests()
    {
        _sut = CreateSut();
    }

    [Fact]
    public void GivenEmptyNameWhenValidatingThenReturnsError()
    {
        var business = new AddBusinessCommand(string.Empty, "testing");

        var result = _sut.Validate(business);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
    }

    [Fact]
    public void GivenEmptyIndustryWhenValidatingThenReturnsError()
    {
        var business = new AddBusinessCommand("testing", string.Empty);

        var result = _sut.Validate(business);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
    }

    [Fact]
    public void GivenValidNameAndIndustryWhenValidatingThenReturnsNoErrors()
    {
        var business = new AddBusinessCommand("testing", "testing");

        var result = _sut.Validate(business);

        result.IsValid.Should().BeTrue();
    }

    private AddBusinessValidator CreateSut() => new();

    public void Dispose()
    {
        
    }
}