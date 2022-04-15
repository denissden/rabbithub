using System.Collections.Concurrent;
using RabbitMQ.Client;
using RabbitHub.Config;
using RabbitHub.Extensions;
using RabbitHub.Consumers;

namespace RabbitHub;
public partial class Hub
{
  public Hub Consume<T>(
    T consumer, QueueConfig queueConfig, 
    bool declareQueue = false, bool bindTopics = false)
    where T: AsyncDefaultBasicConsumer, IConsumer
  {
    var channel = CreateChannel();
    consumer.Model = channel;
    if (declareQueue || queueConfig.AutoDeclare) 
    {
      channel.QDeclare(queueConfig);
    }
    if (bindTopics || queueConfig.AutoBindTopics)
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
