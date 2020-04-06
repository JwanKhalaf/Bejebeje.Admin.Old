namespace Bejebeje.Common
{
  using System;
  using System.Globalization;
  using System.Text;

  public static class StringExtensions
  {
    private static TextInfo _textInfo = new CultureInfo("ku-TR", false).TextInfo;

    public static string Standardize(this string input)
    {
      if (!string.IsNullOrEmpty(input))
      {
        return input.Trim().ToLowerInvariant();
      }

      return string.Empty;
    }

    public static string NormalizeStringForUrl(this string name)
    {
      string lowerCaseName = name.Trim().ToLowerInvariant();
      string normalizedString = lowerCaseName.Normalize(NormalizationForm.FormD);
      StringBuilder stringBuilder = new StringBuilder();

      foreach (char c in normalizedString)
      {
        switch (CharUnicodeInfo.GetUnicodeCategory(c))
        {
          case UnicodeCategory.LowercaseLetter:
          case UnicodeCategory.UppercaseLetter:
          case UnicodeCategory.DecimalDigitNumber:
            stringBuilder.Append(c);
            break;
          case UnicodeCategory.SpaceSeparator:
          case UnicodeCategory.ConnectorPunctuation:
          case UnicodeCategory.DashPunctuation:
            stringBuilder.Append('_');
            break;
        }
      }
      string result = stringBuilder.ToString();
      return string.Join("-", result.Split(new char[] { '_' }
        , StringSplitOptions.RemoveEmptyEntries));
    }

    public static string ToTitleCase(this string value)
    {
      return _textInfo.ToTitleCase(value);
    }
  }
}
