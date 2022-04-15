using RabbitMQ.Client;
using RabbitHub.Handlers;

namespace RabbitHub.Consumers;

public partial class DefaultConsumer : AbstractConsumer 
{
  private Dictionary<string, IHandler> _handlers = new();

  protected override IHandler? GetHandlerForTopic(string? topic)
  {
    return _handlers.TryGetValue(topic ?? "", out var handler) ? handler : null;
  }

  public override IEnumerable<string> GetTopics()
  {
    return _handlers.Keys;
  }
}