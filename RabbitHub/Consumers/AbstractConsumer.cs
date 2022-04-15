using RabbitHub.Handlers;
using RabbitMQ.Client;

namespace RabbitHub.Consumers;
public abstract class AbstractConsumer : AsyncDefaultBasicConsumer, IConsumer
{
  public override Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
  {
    var task = HandleDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);
    return Task.CompletedTask;
  }

  public async Task HandleDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
  {
    var message = new Message();
    message.FillHandlerProperties(
      consumerTag, deliveryTag,
      redelivered, exchange,
      routingKey, properties, body);

    var handler = GetHandlerForTopic(message.Topic);

    IHandleResult? result = default;
    try
    {
      // TODO: remove ugly code
      result = await (handler?.Handle(message) ??
        Task.FromResult<IHandleResult>(HandleResult.Nack()));

      ProcessResult(result, deliveryTag);
    }
    catch (Exception e)
    {
      Console.WriteLine(e.StackTrace);
    }
    finally
    {
      // if (message.IsRpc)
      if (message.Topic?.EndsWith(".rpc") == true)
      {
        SendRpcCallback(result?.Message, properties);
      }
    }
  }

  protected abstract IHandler? GetHandlerForTopic(string? topic);
  
  public abstract IEnumerable<string> GetTopics();

  protected void ProcessResult(IHandleResult result, ulong deliveryTag)
  {
    switch (result.Action)
    {
      case HandleAction.Ack:
        Model.BasicAck(deliveryTag, false);
        break;
      case HandleAction.Nack:
        Model.BasicNack(deliveryTag, false, result.Requeue);
        break;
      default:
        throw new ArgumentException($"Handle action was not handled: {result.Action}");
    }
  }

  protected void SendRpcCallback(Message? message, IBasicProperties props)
  {
    var replyTo = props.ReplyToAddress;
    var respExchange = replyTo.ExchangeName;
    var respTopic = replyTo.RoutingKey;
    var respProps = Model.CreateBasicProperties();
    respProps.CorrelationId = props.CorrelationId;
    var respBody = message?.Body ?? new ReadOnlyMemory<byte>();

    Model.BasicPublish(
      exchange: respExchange,
      routingKey: respTopic,
      basicProperties: respProps,
      body: respBody);
  }
}
