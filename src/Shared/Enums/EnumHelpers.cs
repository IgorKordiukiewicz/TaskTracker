namespace Shared.Enums;

public static class EnumHelpers
{
    public static TEnum GetAllFlags<TEnum>()
        where TEnum : struct, Enum
    {
        int flagsValue = 0;

        foreach(var flag in Enum.GetValues<TEnum>())
        {
            flagsValue |= Convert.ToInt32(flag);
        }

        return (TEnum)(object)flagsValue;
    }
        
}
