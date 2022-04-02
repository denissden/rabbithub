using System.Collections.Concurrent;
using RabbitMQ.Client;
using RabbitHub.Config;
using RabbitHub.Extensions;

namespace RabbitHub;
public partial class Hub
{
  public Hub Consume(
    DefaultConsumer consumer, QueueConfig queueConfig, 
    bool declareQueue = false, bool bindTopics = false)
  {
    var channel = CreateChannel();
    consumer.Model = channel;
    if (declareQueue) 
    {
      channel.QDeclare(queueConfig);
    }
    if (bindTopics)
    {
      foreach (string topic in consumer.GetTopics())
      {
        channel.QueueBind(queueConfig.Name, connConf.Exchange, topic);
      }
    }

    channel.BasicConsume(queueConfig.Name, false, consumer);

    return this;
  }

  public IModel CreateChannel()
  {
    var channel = connection.CreateModel();
    channels.Add(channel);
    return channel;
  }
}
