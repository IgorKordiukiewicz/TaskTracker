using Domain.Common;

namespace UnitTests;

public static class HelperExtensions
{
    public static bool HasDomainEvents<T>(this Entity entity, int count = 1)
        where T : DomainEvent
    {
        return entity.Events.OfType<T>().Count() == count;
    }
}
