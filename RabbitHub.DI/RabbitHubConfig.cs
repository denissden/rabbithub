using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitHub.Config;
using RabbitHub.Handlers;

namespace RabbitHub.DI;

public class RabbitHubConfig
{
  public ConnectionConfig? ConnectionConfig;
  private Action<ConsumerConfig>? _defaultConsumerLambda;
  public ConsumerConfig DefaultConsumerConfig = new ConsumerConfig();

  public RabbitHubConfig Connect(ConnectionConfig config)
  {
    this.ConnectionConfig = config;
    return this;
  }

  public RabbitHubConfig UseDefaultConsumer(Action<ConsumerConfig> config)
  {
    this._defaultConsumerLambda = config;
    return this;
  }

  public void Build(IServiceCollection services)
  {
    if (_defaultConsumerLambda is not null)
    {
      _defaultConsumerLambda(DefaultConsumerConfig);
      DefaultConsumerConfig.Build(services);
    }
  }
}

public class ConsumerConfig
{
  public QueueConfig? QueueConfig;
  public Dictionary<string, Type> HandlerTypes = new();
  public ServiceLifetime? HandlerLifetime;
  public bool DoDeclareQueue = false;
  public bool DoBindTopics = false;
  private ServiceLifetime _defaultHandlerLifetime = ServiceLifetime.Transient;


  public ConsumerConfig Queue(QueueConfig config)
  {
    this.QueueConfig = config;
    return this;
  }

  public ConsumerConfig HandleMessage<T>(string topic) where T : class, IHandler
  {
    HandlerTypes.Add(topic, typeof(T));
    return this;
  }

  public ConsumerConfig HandleRpc<T>(string topic) where T : class, IHandler
  {
    HandlerTypes.Add(topic, typeof(T));
    return this;
  }

  public void Build(IServiceCollection services)
  {
    var lifetime = HandlerLifetime ?? _defaultHandlerLifetime;
    foreach (var kv in HandlerTypes)
    {
      services.TryAdd(new ServiceDescriptor(kv.Value, kv.Value, lifetime));
    }
  }

  public ConsumerConfig DeclareQueue(bool doCreateQueue = true)
  {
    DoDeclareQueue = doCreateQueue;
    return this;
  }

  public ConsumerConfig BindTopics(bool doBindTopics = true)
  {
    DoBindTopics = doBindTopics;
    return this;
  }
}