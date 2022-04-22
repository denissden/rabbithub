namespace RabbitHub.Utils;

public static class Utils
{
  public static Dictionary<string, string> ParseConnectionString(string @string)
  {
    var parts = new Dictionary<string, string>();
    foreach (var val in @string.Split(';'))
    {
      var v = val.Split('=', 2);
      parts.Add(v[0], v[1]);
    }
    return parts;
  }
}