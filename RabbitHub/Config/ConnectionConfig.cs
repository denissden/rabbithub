namespace RabbitHub.Config;

#nullable disable
public class ConnectionConfig
{
  public string AppId { get; set; }
  public string Login { get; set; }
  public string Password { get; set; }
  public string Address { get; set; }
  public string Exchange { get; set; }
  public ushort PrefetchCount { get; set; }

  public static ConnectionConfig GetDefault(string appId)
  {
    return new ConnectionConfig
    {
      AppId = appId,
      Login = "guest",
      Password = "guest",
      Address = "localhost",
      Exchange = "amq.direct",
      PrefetchCount = 100,
    };
  }

  public void FillFromString(string @string)
  {
    var parts = Utils.Utils.ParseConnectionString(@string);

    if (parts.TryGetValue(nameof(AppId), out var a))
      AppId = a;
    if (parts.TryGetValue(nameof(Address), out var ad))
      Address = ad;
    if (parts.TryGetValue(nameof(Login), out var l))
      Login = l;
    if (parts.TryGetValue(nameof(Password), out var p))
      Password = p;
    if (parts.TryGetValue(nameof(Exchange), out var e))
      Exchange = e;
  }
}