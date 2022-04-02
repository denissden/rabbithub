using System.Text.Json;

namespace RabbitMQ.Hub.Handlers;
public abstract class RpcHandler<TResult> : IHandler, IRpcHandler
{
  public abstract Task<IHandleResult> Handle(Message message);

  public IHandleResult Ack(TResult result)
  {
    byte[]? respBody = default(byte[]);
    if (result != null)
      respBody = JsonSerializer.SerializeToUtf8Bytes(result);
    return HandleResult.RpcAck(new Message(respBody));
  }

  public IHandleResult Nack()
  {
    return HandleResult.RpcNack(null);
  }
}

/// <summary>
/// Marker interface
/// </summary>
public interface IRpcHandler { }