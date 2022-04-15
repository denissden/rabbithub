using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitHub.Handlers;
using RabbitHub.Config;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitHub.Consumers;
public partial class HostConsumer
{
  # nullable disable
  public class HostConsumerConfigurator : IConsumerConfigurator
  {
    public Dictionary<string, Type> HandlerTypes { get; set; } = new();
    public QueueConfig QueueConfig { get; set; }
    private IServiceCollection _serviceCollection;

    public HostConsumerConfigurator(IServiceCollection services)
    {
      _serviceCollection = services;
    }

    public IConsumerConfigurator HandleMessage<T>(string topic) where T : class, IHandler
    {
      _serviceCollection.AddScoped<T>();
      HandlerTypes.Add(topic, typeof(T));
      return this;
    }

    public IConsumerConfigurator HandleRpc<T>(string topic) where T : class, IHandler
    {
      return HandleMessage<T>(topic);
    }
  }

  public void ApplyConfiguration(HostConsumerConfigurator configurator)
  {
    foreach (var kv in configurator.HandlerTypes)
    {
      _handlerTypes.Add(kv.Key, kv.Value);
    }
  }
}
