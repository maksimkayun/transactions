using Common.Settings;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Transactions.EsbAdapter.EventBus;

public class KafkaProducerService
{
    private readonly AppSettings _options;
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerService(AppSettings options)
    {
        _options = options;
        var producerconfig = new ProducerConfig
        {
            BootstrapServers = _options.KafkaSettings.BootstrapServers
        };

        _producer = new ProducerBuilder<Null, string>(producerconfig).Build();
    }

    public async Task ProduceAsync(string message, CancellationToken cancellationToken)
    {
        var kafkamessage = new Message<Null, string> { Value = message };
        await _producer.ProduceAsync(_options.KafkaSettings.TopicName, kafkamessage, cancellationToken);
    }
}