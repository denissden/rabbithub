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

  public void FillFromString(string @string)
  {
    var parts = Utils.Utils.ParseConnectionString(@string);

    const string trueString = "true";
    if (parts.TryGetValue(nameof(Name), out var a))
      Name = a;
    if (parts.TryGetValue(nameof(Durable), out var durable))
      Durable = durable.ToLower() == trueString;
    if (parts.TryGetValue(nameof(Exclusive), out var exclusive))
      Exclusive = exclusive.ToLower() == trueString;
    if (parts.TryGetValue(nameof(AutoDelete), out var delete))
      AutoDelete = delete.ToLower() == trueString;
    if (parts.TryGetValue(nameof(AutoDeclare), out var declare))
      AutoDeclare = declare.ToLower() == trueString;
    if (parts.TryGetValue(nameof(AutoBindTopics), out var bind))
      AutoBindTopics = bind.ToLower() == trueString;
  }
}
