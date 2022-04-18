using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitHub.Config;
using RabbitHub.Consumers;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitHub.DI;
public class RabbitService : IHostedService
{
  private readonly Hub _hub;
  private readonly RabbitHubConfig _config;
  private readonly IServiceProvider _serviceProvider;
  public RabbitService(Hub hub, RabbitHubConfig config, IServiceProvider serviceProvider)
  {
    _hub = hub;
    _config = config;
    _serviceProvider = serviceProvider;
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    BuildHub();
    return Task.CompletedTask;
  }

  public void BuildHub()
  {
    if (_config.DefaultConsumerConfig is { } consConfig)
    {
      var cons = _serviceProvider.GetRequiredService<HostConsumer>();
      cons.Build(consConfig);
      _hub.Consume(
        cons,
        consConfig.QueueConfig!,
        declareQueue: consConfig.DoDeclareQueue,
        bindTopics: consConfig.DoBindTopics);
    }
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {

    return Task.CompletedTask;
  }
}
