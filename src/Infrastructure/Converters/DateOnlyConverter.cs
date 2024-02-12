using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Converters;

internal class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter()
        : base(x => x.ToDateTime(TimeOnly.MinValue), 
            x => DateOnly.FromDateTime(x))
    { }
}