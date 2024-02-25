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
}