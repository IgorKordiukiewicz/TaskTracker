using MudBlazor;

namespace Web.Client.Common;

public static class AvatarColorGenerator
{
    private static readonly string[] _colors = new[]
    {
        Colors.Red.Lighten1, Colors.Pink.Lighten1, Colors.Purple.Lighten1, Colors.DeepPurple.Lighten1,
        Colors.Indigo.Lighten1, Colors.Blue.Lighten1, Colors.Teal.Lighten1, Colors.Green.Lighten1,
        Colors.LightGreen.Lighten1, Colors.Lime.Lighten1
    };

    public static string Generate()
    {
        var random = new Random();
        return _colors[random.Next(_colors.Length)];
    }
}

