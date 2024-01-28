namespace Excalibur.Authentication;

public static class StringExtensions
{
    public static string TrimChallenge(this string value)
    {
        return value
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
