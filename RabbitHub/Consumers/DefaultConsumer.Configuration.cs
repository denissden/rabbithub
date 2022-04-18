using RabbitMQ.Client;
using RabbitHub.Handlers;
using RabbitHub.Config;

namespace RabbitHub.Consumers;

public partial class DefaultConsumer
{
  public DefaultConsumer HandleMessage<T>(string topic) where T : class, IHandler
  {
    var consumer = Activator.CreateInstance<T>();
    Handle(consumer, topic);
    return this;
  }

  public DefaultConsumer HandleRpc<T>(string topic) where T : class, IHandler
  {
    return HandleMessage<T>(topic);
  }

  public void Handle(IHandler handler, string topic)
  {
    _handlers.Add(topic, handler);
  }
}