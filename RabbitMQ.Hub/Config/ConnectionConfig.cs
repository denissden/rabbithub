namespace RabbitMQ.Hub.Config;

#nullable disable
public class ConnectionConfig
{
  public string AppId { get; set; }
  public string Login { get; set; }
  public string Password { get; set; }
  public string Address { get; set; }
  public string Exchange { get; set; }

  public static ConnectionConfig GetDefault(string appId) 
  {
    return new ConnectionConfig{
      AppId = appId,
      Login = "guest",
      Password = "guest",
      Address = "localhost",
      Exchange = "amq.direct"
    };
  } 
}