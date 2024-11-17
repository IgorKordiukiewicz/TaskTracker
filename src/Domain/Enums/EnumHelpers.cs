namespace Domain.Enums;

public static class EnumHelpers
{
    public static TEnum GetAllFlags<TEnum>()
        where TEnum : struct, Enum
    {
        var flagsValue = Enum.GetValues<TEnum>()
            .Aggregate(0, (current, flag) => current | Convert.ToInt32(flag));

        return (TEnum)(object)flagsValue;
    }
        
}
