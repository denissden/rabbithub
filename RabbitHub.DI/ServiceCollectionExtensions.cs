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
    ConnectionConfig connectionConfig,
    Action<HostConsumer.HostConsumerConfigurator> configureConsumer,
    QueueConfig? queueConfig = null)
  {
    var hub = Hub.Connect(connectionConfig);
    services.AddSingleton<Hub>(hub);

    var configurator = new HostConsumer.HostConsumerConfigurator(services);
    configureConsumer(configurator);
    queueConfig = queueConfig ?? QueueConfig.Create(connectionConfig.DefaultQueue);
    configurator.QueueConfig = queueConfig;
    services.AddSingleton(configurator);
    services.AddSingleton<HostConsumer>();

    services.AddHostedService<RabbitService>();

    return services;
  }
}
