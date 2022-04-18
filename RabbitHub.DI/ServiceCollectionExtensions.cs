using RabbitHub;
using RabbitHub.DI;
using RabbitHub.Config;
using RabbitHub.Consumers;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddRabbitHub(
    this IServiceCollection services,
    Action<RabbitHubConfig> configureRabbit)
  {
    var rabbitConfig = new RabbitHubConfig();
    configureRabbit(rabbitConfig);
    rabbitConfig.Build(services);

    services.AddSingleton<RabbitHubConfig>(rabbitConfig);

    services.AddHostedService<RabbitService>();

    var rabbitHub = BuildHub(rabbitConfig);
    services.AddSingleton(rabbitHub);

    if (rabbitConfig.DefaultConsumerConfig is { } consConfig)
    {
      if (consConfig.QueueConfig is null)
        throw new ArgumentException($"{nameof(QueueConfig)} was null");
      services.AddSingleton<HostConsumer>();
    }

    return services;
  }

  private static Hub BuildHub(RabbitHubConfig config)
  {
    var connConf = config.ConnectionConfig ??
      throw new ArgumentException($"{nameof(ConnectionConfig)} was not specified");
    var rabbitHub = Hub.Connect(connConf);

    return rabbitHub;
  }
}
