using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitHub.Config;
using Examples.DI;

var conf = ConnectionConfig.GetDefault("test");
var queueConf = QueueConfig.Create(conf.DefaultQueue);
queueConf.AutoDeclare = true;
queueConf.AutoBindTopics = true;

var host = Host
  .CreateDefaultBuilder()
  .ConfigureServices((context, services) =>
  {
    services.AddRabbitHub(hub => 
      hub
      .Connect(conf)
      .UseDefaultConsumer(cons => 
        cons
        .Queue(queueConf)
        .DeclareQueue()
        .BindTopics()
        .HandleRpc<NumberRpcHandler>("test.number.rpc")
      )
    );
  });

await host.Build().RunAsync();