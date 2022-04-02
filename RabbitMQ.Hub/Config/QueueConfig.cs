namespace RabbitMQ.Hub.Config;

#nullable disable
public class QueueConfig
{
  public string Name { get; set; }
  public bool Durable { get; set; }
  public bool Exclusive { get; set; }
  public bool AutoDelete { get; set; }
  public IDictionary<string, object> Arguments { get; set; }

  public static QueueConfig Create(string name = "")
  {
    return new QueueConfig 
    {
      Name = name,
      Durable = false,
      Exclusive = false,
      AutoDelete = true,
    };
  }
}
