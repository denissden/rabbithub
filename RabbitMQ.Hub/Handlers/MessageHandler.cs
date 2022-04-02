using System.Text.Json;

namespace RabbitMQ.Hub.Handlers;
public abstract class MessageHandler<TResult> : IHandler
{
  public abstract Task<IHandleResult> Handle(Message message);

  public IHandleResult Ack(TResult result)
  {
    byte[]? respBody = default(byte[]);
    if (result != null)
      respBody = JsonSerializer.SerializeToUtf8Bytes(result);
    return HandleResult.RpcAck(new Message(respBody));
  }

  public IHandleResult Nack(TResult result)
  {
    return HandleResult.RpcNack(null);
  }
}
