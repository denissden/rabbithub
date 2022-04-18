using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitHub.DI;
using RabbitHub.Handlers;

namespace RabbitHub.Consumers;
public partial class HostConsumer : AbstractConsumer
{
  private Dictionary<string, Type> _handlerTypes = new();
  private readonly IServiceProvider _serviceProvider;
  public HostConsumer(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }
  protected override IHandler? GetHandlerForTopic(string? topic)
  {
    if (topic is null) return null;
    if (_handlerTypes.TryGetValue(topic, out var type))
    {
      return _serviceProvider.GetService(type) as IHandler;
    }
    return null;
  }

  public override IEnumerable<string> GetTopics()
  {
    return _handlerTypes.Keys;
  }

  public void Build(ConsumerConfig config)
  {
    _handlerTypes = config.HandlerTypes;
  }
}
