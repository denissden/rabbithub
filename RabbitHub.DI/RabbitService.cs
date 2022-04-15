using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitHub.Config;
using RabbitHub.Consumers;

namespace RabbitHub.DI;
public class RabbitService : IHostedService
{
  private readonly Hub _rabbitHub;
  private readonly HostConsumer.HostConsumerConfigurator _consumerConfigurator;
  private readonly HostConsumer _consumer;

  public RabbitService(
    Hub rabbitHub,
    HostConsumer.HostConsumerConfigurator consumerConfigurator,
    HostConsumer consumer)
  {
    _rabbitHub = rabbitHub;
    _consumerConfigurator = consumerConfigurator;
    _consumer = consumer;
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _consumer.ApplyConfiguration(_consumerConfigurator);
    _rabbitHub.Consume(_consumer, _consumerConfigurator.QueueConfig);
    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _rabbitHub.Close();
    return Task.CompletedTask;
  }
}
