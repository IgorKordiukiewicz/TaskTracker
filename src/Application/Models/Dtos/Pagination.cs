using FluentValidation;

namespace Application.Models.Dtos;

public class Pagination
{
    public int PageNumber { get; set; } = 1;
    public int ItemsPerPage { get; set; } = 10;

    public int Offset => (PageNumber - 1) * ItemsPerPage;
    public int GetPagesCount(int itemsCount) => (itemsCount - 1) / ItemsPerPage + 1;
}

public class PaginationValidator : AbstractValidator<Pagination>
{
    public PaginationValidator()
    {
        RuleFor(x => x.PageNumber).NotNull().GreaterThanOrEqualTo(1);
        RuleFor(x => x.ItemsPerPage).NotNull().GreaterThanOrEqualTo(1);
    }
}