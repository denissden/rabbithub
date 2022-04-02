using System.Text.Json;

namespace RabbitHub.Handlers;
public class LambdaHandler : IHandler, ILabmdaHandlerArgs
{
  private Func<Message, ILabmdaHandlerArgs, Task<IHandleResult>>? _lambda;
  public Func<Message, ILabmdaHandlerArgs, Task<IHandleResult>> Lambda
  {
    set { _lambda = value; }
  }

  public Task<IHandleResult> Handle(Message message)
  {
    return _lambda?.Invoke(message, this) ?? Task.FromResult(Nack());
  }

  public IHandleResult Ack(object result)
  {
    byte[]? respBody = default(byte[]);
    if (result != null)
      respBody = JsonSerializer.SerializeToUtf8Bytes(result);
    return HandleResult.RpcAck(new Message(respBody));
  }

  public IHandleResult Ack()
  {
    return HandleResult.Ack();
  }

  public IHandleResult Nack()
  {
    return HandleResult.RpcNack(null);
  }
}

public interface ILabmdaHandlerArgs
{
  IHandleResult Nack();
  IHandleResult Ack();
  IHandleResult Ack(object result);
}