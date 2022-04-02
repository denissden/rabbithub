namespace RabbitMQ.Hub.Config;
public class ExchangeConfig
{
  string Name { get; set; } = "";
  ExchageType Type { get; set; }
  string TypeString => this.Type.ToString();
  bool Durable { get; set; } = false;
  bool Exclusive { get; set; } = false;
  bool AutoDelete { get; set; } = true;
  IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
}

public enum ExchageType
{
  Direct, Topic, Fanout, Headers
}