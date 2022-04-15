namespace RabbitHub.Config;

#nullable disable
public class QueueConfig
{
  public string Name { get; set; }
  public bool Durable { get; set; }
  public bool Exclusive { get; set; }
  public bool AutoDelete { get; set; }

  /// <summary>
  /// Declare a queue if it does not exist. Custom RabbitHub property. 
  /// </summary>
  /// <value></value>
  public bool AutoDeclare { get; set; }
  /// <summary>
  /// Auto topics to a queue when starting consuming. Custom RabbitHub property. 
  /// </summary>
  /// <value></value>
  public bool AutoBindTopics { get; set; }
  public IDictionary<string, object> Arguments { get; set; }

  public static QueueConfig Create(string name = "")
  {
    return new QueueConfig
    {
      Name = name,
      Durable = false,
      Exclusive = false,
      AutoDelete = true,
      AutoDeclare = false,
      AutoBindTopics = false
    };
  }
}
