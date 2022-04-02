using System.Collections.Concurrent;
using RabbitMQ.Client;
using RabbitMQ.Hub.Config;

namespace RabbitMQ.Hub;


public record DuplexModel(IModel Send, IModel Receive);
public partial class RabbitHub
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
  public RabbitHub(ConnectionConfig connectionConfig)
  {
    connConf = connectionConfig;
  }
#nullable enable
}
