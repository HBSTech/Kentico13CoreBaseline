
public static class StringExtensions
{

    public static Maybe<string> AsNullOrWhitespaceMaybe(this string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return Maybe.From(value);
        }
        else
        {
            return Maybe<string>.None;
        }
    }

    public static IEnumerable<string> SplitAndRemoveEntries(this string value, string delimiters = "|,;")
    {
        return value.Split(delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    }

    public static string RemoveTildeFromFirstSpot(this string value) =>
        value.Length > 0 && value[0] == '~'
        ? value.Substring(1) : value;

    public static string MaxLength(this string value, int maxLength, string elipses = "...")
    {
        if (value.Length > maxLength)
        {
            return string.Concat(value.AsSpan(0, maxLength), elipses);
        }
        return value;
    }
}
