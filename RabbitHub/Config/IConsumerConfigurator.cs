using RabbitHub.Handlers;

namespace RabbitHub.Config;
public interface IConsumerConfigurator
{
  IConsumerConfigurator HandleMessage<T>(string topic) where T: class, IHandler;
  IConsumerConfigurator HandleRpc<T>(string topic) where T: class, IHandler;
}
