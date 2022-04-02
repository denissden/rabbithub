using RabbitMQ.Client;
using RabbitMQ.Hub.Handlers;

namespace RabbitMQ.Hub;

public partial class DefaultConsumer
{
  public void HandleRpc<T>(T handler, string topic)
    where T : IHandler, IRpcHandler
  {
    Handle(handler, topic);
  }

  public void Handle(IHandler handler, string topic)
  {
    _handlers.Add(topic, handler);
  }

  public IEnumerable<string> GetTopics(){
    return _handlers.Keys;
  }
}