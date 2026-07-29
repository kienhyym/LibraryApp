using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LibraryApp.Helpers;

public static class SlugHelper
{
    public static string Generate(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "book";

        // bỏ dấu tiếng Việt
        text = RemoveDiacritics(text);

        text = text.ToLowerInvariant();

        // thay khoảng trắng bằng -
        text = Regex.Replace(text, @"\s+", "-");

        // chỉ giữ a-z 0-9 -
        text = Regex.Replace(text, @"[^a-z0-9\-]", "");

        // bỏ nhiều dấu -
        text = Regex.Replace(text, @"-+", "-");

        return text.Trim('-');
    }

    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);

        var builder = new StringBuilder();

        foreach (char c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace('đ', 'd')
            .Replace('Đ', 'D');
    }
}