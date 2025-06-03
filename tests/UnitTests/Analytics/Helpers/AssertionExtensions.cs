using Analytics.Infrastructure.Models;

namespace UnitTests.Analytics.Helpers;

public static class AssertionExtensions
{
    public static void ShouldHaveCount<TProjection>(this TProjection? projection, int expected)
        where TProjection : IDailyCountProjection
    {
        projection.Should().NotBeNull();
        projection!.Count.Should().Be(expected);
    }
}