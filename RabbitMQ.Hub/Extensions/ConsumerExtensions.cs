
using RabbitMQ.Hub.Handlers;

namespace RabbitMQ.Hub.Extensions;

public static class ConsumerExtensions
{
  public static void Handle(this DefaultConsumer consumer, string topic, Func<Message, ILabmdaHandlerArgs, Task<IHandleResult>> lambda)
  {
    var handler = new LambdaHandler();
    handler.Lambda = lambda;
    consumer.Handle(handler, topic);
  }
}
