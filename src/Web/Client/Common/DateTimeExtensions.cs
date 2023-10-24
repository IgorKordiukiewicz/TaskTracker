using System.Globalization;

namespace Web.Client.Common;

public static class DateTimeExtensions
{
    public static string GetHumanReadableTimeDifference(this DateTime date, DateTime now, CultureInfo cultureInfo)
    {
        if(date > now)
        {
            throw new ArgumentException($"{nameof(date)} has to be before {nameof(now)}.");
        }

        var diff = now - date;
        return diff.TotalSeconds switch
        {
            < 10 => "just now",
            < 60 => "few seconds ago",
            < 120 => "a minute ago",
            < 60 * 60 => $"{(int)diff.TotalMinutes} minutes ago",
            < 60 * 60 * 2 => "an hour ago",
            < 60 * 60 * 24 => $"{(int)diff.TotalHours} hours ago",
            _ => date.ToString("dd.MM.yyyy", cultureInfo)
        };
    }

    public static string GetHumanReadableTimeDifference(this DateTime date, DateTime now)
        => date.GetHumanReadableTimeDifference(now, CultureInfo.InvariantCulture);
}
