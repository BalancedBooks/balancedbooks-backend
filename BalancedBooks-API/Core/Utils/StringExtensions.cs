namespace BalancedBooksAPI.Core.Utils;

public static class StringExtensions
{
    public static string DecodeBase64(this string value)
    {
        var valueBytes = Convert.FromBase64String(value);
        return System.Text.Encoding.UTF8.GetString(valueBytes);
    }
}