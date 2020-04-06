namespace Bejebeje.Common
{
  public static class BoolExtensions
  {
    public static string ToYesOrNo(this bool value)
    {
      return value ? "Yes" : "No";
    }
  }
}
