using RabbitMQ.Client;
using RabbitMQ.Hub.Config;

namespace RabbitMQ.Hub.Extensions;
public static class ModelExtensions
{
  public static QueueDeclareOk QDeclare(this IModel channel, QueueConfig conf)
  {
    return channel.QueueDeclare(
      queue: conf.Name,
      durable: conf.Durable,
      exclusive: conf.Exclusive,
      autoDelete: conf.AutoDelete,
      arguments: conf.Arguments
    );
  }
}
