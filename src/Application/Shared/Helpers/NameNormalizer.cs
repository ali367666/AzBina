namespace Application.Shared.Helpers;

public static class NameNormalizer
{
    public static string NormalizeName(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // əvvəl/son boşluqları sil + aradakı çoxlu boşluğu 1-ə endir
        var cleaned = string.Join(" ",
            input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));

        cleaned = cleaned.ToLowerInvariant();

        // 1-ci hərfi böyük et
        return char.ToUpperInvariant(cleaned[0]) + cleaned[1..];
    }
}
