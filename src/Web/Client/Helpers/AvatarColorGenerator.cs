namespace Web.Client.Common;

public static class AvatarColorGenerator
{
    private static readonly string[] Colors = new[]
    {
        MudBlazor.Colors.Red.Lighten1, MudBlazor.Colors.Pink.Lighten1, MudBlazor.Colors.Purple.Lighten1, MudBlazor.Colors.DeepPurple.Lighten1,
        MudBlazor.Colors.Indigo.Lighten1, MudBlazor.Colors.Blue.Lighten1, MudBlazor.Colors.Teal.Lighten1, MudBlazor.Colors.Green.Lighten1,
        MudBlazor.Colors.LightGreen.Lighten1, MudBlazor.Colors.Lime.Lighten1
    };

    public static string Generate()
    {
        var random = new Random();
        return Colors[random.Next(Colors.Length)];
    }
}

