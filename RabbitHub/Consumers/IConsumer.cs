using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitHub.Consumers;
public interface IConsumer
{
  IModel Model { get; set; }
  Task HandleDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body);
  IEnumerable<string> GetTopics();
}
