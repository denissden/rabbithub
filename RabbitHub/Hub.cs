using System.Collections.Concurrent;
using RabbitMQ.Client;
using RabbitHub.Config;

namespace RabbitHub;


public record DuplexModel(IModel Send, IModel Receive);
public partial class Hub
{
  public string AppId => connConf?.AppId ?? "";
  private ConnectionConfig connConf;
  private IConnection connection;
  private IModel messageChannel;
  private DuplexModel rpcChannel;
  private List<IModel> channels = new();
  private IBasicConsumer rpcCallbackConsumer;
  private string callbackQueue;
  private string callbackTopic => callbackQueue + ".topic";

  private ConcurrentDictionary<Guid, TaskCompletionSource<Message>> rpcWaitingCallback = new();

#nullable disable
  public Hub(ConnectionConfig connectionConfig)
  {
    connConf = connectionConfig;
  }
#nullable enable
}
