using Shared.Dtos;

namespace UnitTests.Shared;

public class PaginationTests
{
    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(2, 10, 10)]
    [InlineData(3, 10, 20)]
    public void GetOffset_Should_CalculateOffsetCorrectly(int pageNumber, int itemsPerPage, int expectedResult)
    {
        var pagination = new Pagination
        {
            PageNumber = pageNumber,
            ItemsPerPage = itemsPerPage
        };
        var result = pagination.Offset;

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(0, 10, 1)]
    [InlineData(9, 10, 1)]
    [InlineData(10, 10, 1)]
    [InlineData(11, 10, 2)]
    [InlineData(19, 10, 2)]
    public void GetPagesCount_Should_CalculatePagesCountCorrectly(int itemsCount, int itemsPerPage, int expectedResult)
    {
        var pagination = new Pagination
        {
            PageNumber = 1,
            ItemsPerPage = itemsPerPage
        };
        var result = pagination.GetPagesCount(itemsCount);

        result.Should().Be(expectedResult);
    }
}
