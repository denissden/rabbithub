using RabbitMQ.Client;
namespace RabbitHub.Extensions;
public static class BasicPropertiesExtensions
{
  public static void FillFromMessage(this IBasicProperties props, Message message)
  {
    props.Headers = message.Headers;
    props.CorrelationId = message.CorrelationId;
  }
}