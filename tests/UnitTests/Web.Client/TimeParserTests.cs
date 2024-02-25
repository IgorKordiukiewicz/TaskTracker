using Web.Client.Common;

namespace UnitTests.Web.Client;

public class TimeParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("5h 3d")] // wrong order
    [InlineData("1d 15h -5m")] // negative numbers
    public void TryToMinutes_ShouldFail_WhenInputIsNotCorrect(string input)
    {
        var success = TimeParser.TryToMinutes(input, out _);

        success.Should().BeFalse();
    }
    
    [Theory]
    [InlineData("2", 120)]
    [InlineData("2d 5h 40m", 3220)]
    [InlineData("3d", 4320)]
    [InlineData("4h", 240)]
    [InlineData("80m", 80)]
    [InlineData("1d 2h", 1560)]
    [InlineData("1d 15m", 1455)]
    [InlineData("8h 100m", 580)]
    public void TryToMinutes_ShouldSucceedAndParseCorrectly_WhenInputIsCorrect(string input, int expectedMinutes)
    {
        var success = TimeParser.TryToMinutes(input, out var result);

        using (new AssertionScope())
        {
            success.Should().BeTrue();
            result.Should().Be(expectedMinutes);
        }
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(0)]
    public void FromMinutes_ShouldReturnEmptyString_WhenInputIsZeroOrNegative(int input)
    {
        var result = TimeParser.FromMinutes(input);

        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(45, "45m")]
    [InlineData(150, "2h 30m")]
    [InlineData(120, "2h")]
    [InlineData(1440, "1d")]
    [InlineData(1635, "1d 3h 15m")]
    public void FromMinutes_ShouldReturnCorrectString_WhenInputIsPositive(int input, string expected)
    {
        var result = TimeParser.FromMinutes(input);

        result.Should().Be(expected);
    }
}