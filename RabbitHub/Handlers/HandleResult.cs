namespace RabbitHub.Handlers;

public interface IHandleResult
{
  Message? Message { get; }
  HandleAction Action { get; }
  bool SendRpcCallback { get; }
  bool Requeue { get; }
}

public record HandleResult(
  Message? Message = null,
  HandleAction Action = HandleAction.Nack,
  bool SendRpcCallback = false,
  bool Requeue = false) : IHandleResult
{
  public static HandleResult Ack() =>
    new HandleResult(Action: HandleAction.Ack);
  public static HandleResult Nack() =>
    new HandleResult(Action: HandleAction.Nack);
  public static HandleResult RpcAck(Message message) =>
    new HandleResult(Action: HandleAction.Ack, SendRpcCallback: true, Message: message);
  public static HandleResult RpcNack(Message? message) =>
    new HandleResult(Action: HandleAction.Nack, SendRpcCallback: true, Message: message);
};
