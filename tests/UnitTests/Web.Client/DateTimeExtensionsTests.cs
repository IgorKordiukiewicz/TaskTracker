using Web.Client.Common;

namespace UnitTests.Web.Client;

public class DateTimeExtensionsTests
{
    public static IEnumerable<object[]> GetHumanReadableTimeDifferenceData => new List<object[]>
    {
        new object[] { new DateTime(2023, 10, 15, 12, 30, 30), "just now" },
        new object[] { new DateTime(2023, 10, 15, 12, 30, 21), "just now" },
        new object[] { new DateTime(2023, 10, 15, 12, 30, 20), "few seconds ago" },
        new object[] { new DateTime(2023, 10, 15, 12, 29, 31), "few seconds ago" },
        new object[] { new DateTime(2023, 10, 15, 12, 29, 30), "a minute ago" },
        new object[] { new DateTime(2023, 10, 15, 12, 28, 31), "a minute ago" },
        new object[] { new DateTime(2023, 10, 15, 12, 28, 30), "2 minutes ago" },
        new object[] { new DateTime(2023, 10, 15, 11, 31, 30), "59 minutes ago" },
        new object[] { new DateTime(2023, 10, 15, 11, 30, 30), "an hour ago" },
        new object[] { new DateTime(2023, 10, 15, 10, 30, 31), "an hour ago" },
        new object[] { new DateTime(2023, 10, 15, 10, 30, 30), "2 hours ago" },
        new object[] { new DateTime(2023, 10, 14, 13, 30, 30), "23 hours ago" },
        new object[] { new DateTime(2023, 10, 14, 12, 30, 30), "14.10.2023" },
        new object[] { new DateTime(2023, 10, 12, 12, 31, 30), "12.10.2023" },
        new object[] { new DateTime(2023, 5, 12, 12, 31, 30), "12.05.2023" },
    };

    [Theory]
    [MemberData(nameof(GetHumanReadableTimeDifferenceData))]
    public void GetHumanReadableTimeDifference_ShouldReturnCorrectString(DateTime date, string expected)
    {
        var now = new DateTime(2023, 10, 15, 12, 30, 30);

        var result = date.GetHumanReadableTimeDifference(now);

        result.Should().Be(expected);
    }

    [Fact]
    public void GetHumanReadableTimeDifference_ShouldThrow_WhenDateIsAfterNow()
    {
        var now = new DateTime(2023, 10, 15);
        var date = now.AddDays(1);

        Action act = () => date.GetHumanReadableTimeDifference(now);

        act.Should().Throw<ArgumentException>();
    }
}
