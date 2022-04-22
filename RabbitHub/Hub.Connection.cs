using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitHub.Config;
using RabbitHub.Extensions;

namespace RabbitHub;

public partial class Hub
{
  public static Hub Connect(ConnectionConfig connectionConfig)
  {
    var hub = new Hub(connectionConfig);

    var connFactory = new ConnectionFactory
    {
      HostName = connectionConfig.Address,
      UserName = connectionConfig.Login,
      Password = connectionConfig.Password,
      // IMPORTANT: support async consumer
      DispatchConsumersAsync = true,
      ConsumerDispatchConcurrency = Environment.ProcessorCount
    };

    var conn = hub.connection = connFactory.CreateConnection(connectionConfig.AppId);
    hub.messageChannel = conn.CreateModel();

    #region rpc
    var rpcSend = conn.CreateModel();
    var rpcReceive = conn.CreateModel();
    hub.rpcChannel = new DuplexModel(rpcSend, rpcReceive);

    var receiveConf = QueueConfig.Create("");
    receiveConf.Exclusive = true;
    var callbackQueue = rpcReceive.QDeclare(receiveConf).QueueName;
    hub.callbackQueue = callbackQueue;

    rpcReceive.QueueBind(
      hub.callbackQueue,
      exchange: hub.connConf.Exchange,
      hub.callbackTopic);

    var callbackCons = new AsyncEventingBasicConsumer(rpcReceive);
    hub.rpcCallbackConsumer = callbackCons;
    rpcReceive.BasicConsume(callbackQueue, false, callbackCons);
    callbackCons.Received += hub.OnRpcReceived;
    rpcSend.BasicNacks += hub.OnRpcNacks;
    rpcSend.BasicReturn += hub.OnRpcReturn;
    #endregion

    #region qos
    rpcReceive.BasicQos(
      prefetchSize: 0,
      prefetchCount: connectionConfig.PrefetchCount,
      global: false);
    #endregion

    return hub;
  }

  private Task OnRpcReceived(object sender, BasicDeliverEventArgs args)
  {
    var correlationId = Guid.Parse(args.BasicProperties.CorrelationId);
    var key = new UlongAndGuid(0, correlationId);
    bool found = rpcWaitingCallback.TryRemove(key, out var tcs);
    if (!found)
    {
      rpcChannel.Receive.BasicNack(args.DeliveryTag, false, false);
      return Task.CompletedTask;
    }

    var result = new Message().FillEventArgs(args);
    tcs!.SetResult(result);
    rpcChannel.Receive.BasicAck(args.DeliveryTag, false);

    return Task.CompletedTask;
  }

  // handle rpc errors
  private void OnRpcNacks(object? sender, BasicNackEventArgs args)
  {
    var key = new UlongAndGuid(args.DeliveryTag, Guid.Empty);
    if (rpcWaitingCallback.TryRemove(key, out var tcs))
    {
      tcs.SetResult(null);
    }
  }

  // message was unroutable
  private void OnRpcReturn(object? sender, BasicReturnEventArgs args)
  {
    var correlationId = Guid.Parse(args.BasicProperties.CorrelationId);
    var key = new UlongAndGuid(0, correlationId);
    if (rpcWaitingCallback.TryRemove(key, out var tcs))
    {
      tcs.SetResult(null);
    }
  }

  public void Close()
  {
    connection.Close();
  }
}
