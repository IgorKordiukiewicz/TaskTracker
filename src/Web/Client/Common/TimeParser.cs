using System.Text.RegularExpressions;

namespace Web.Client.Common;

public static class TimeParser
{
    public static bool TryToMinutes(string input, out int result)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            result = 0;
            return false;
        }

        // Treat integer input as hours ?
        var isInteger = int.TryParse(input, out var integer);
        if (isInteger)
        {
            result = integer * 60;
            return true;
        }

        // match negative numbers too to so method can fail instead of ignoring the group with it
        const string regex = @"((-*\d+d)? *(-*\d+h)? *(-*\d+m)? *)";
        var match = Regex.Match(input, regex);

        // fail when match is shorter than input to prevent inputs in wrong order
        if (!match.Success || match.Value.Length < input.Length)
        {
            result = 0;
            return false;
        }

        var days = match.Groups[2].Value;
        var hours = match.Groups[3].Value;
        var minutes = match.Groups[4].Value;
        
        // fail if didn't match any groups or any group has a negative value
        if((string.IsNullOrEmpty(days) && string.IsNullOrEmpty(hours) && string.IsNullOrEmpty(minutes))
           || days.StartsWith('-') || hours.StartsWith('-') || minutes.StartsWith('-'))
        {
            result = 0;
            return false;
        }

        result = (ParseGroup(days) * 24 * 60) + (ParseGroup(hours) * 60) + ParseGroup(minutes);
        return true;
    }

    public static string FromMinutes(int totalMinutes)
    {
        if (totalMinutes < 0)
        {
            return string.Empty;
        }

        if (totalMinutes == 0)
        {
            return "0h";
        }
        
        var days = totalMinutes / (24 * 60);
        var hours = (totalMinutes % (24 * 60)) / 60;
        var minutes = totalMinutes % 60;

        var result = string.Empty;
        if (days > 0)
        {
            result += $" {days}d";
        }
        if (hours > 0)
        {
            result += $" {hours}h";
        }
        if (minutes > 0)
        {
            result += $" {minutes}m";
        }

        return result.Trim();
    }
    
    private static int ParseGroup(string group)
        => !string.IsNullOrEmpty(group) ? int.Parse(group[..^1]) : 0;
}