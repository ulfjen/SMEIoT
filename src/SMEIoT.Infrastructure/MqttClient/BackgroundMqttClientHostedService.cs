using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;

namespace SMEIoT.Infrastructure.MqttClient
{
  public class BackgroundMqttClientHostedService : IHostedService, IDisposable
  {
    public IManagedMqttClient MqttClient { get; }

    private readonly ManagedMqttClientOptions _options;
    private readonly string[] _topicFilters;

    public BackgroundMqttClientHostedService(
        ManagedMqttClientOptions options,
        string[] topicFilters)
    {
      MqttClient = new MqttFactory().CreateManagedMqttClient();
      _options = options;
      _topicFilters = topicFilters;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      await Task.WhenAll(_topicFilters.Select(f => MqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(f).Build())));
      MqttClient.ApplicationMessageReceivedHandler = new MessageHandler();
      await MqttClient.StartAsync(_options);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      await MqttClient.StopAsync();
    }

    public class MessageHandler : IMqttApplicationMessageReceivedHandler
    {
      public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
      {
        Console.WriteLine(eventArgs.ApplicationMessage);
        if (eventArgs.ApplicationMessage.Topic == "name_of_desired_topic")
        {
          // Handle event
        }
      }
    }

    public void Dispose()
    {
    }
  }
}
