namespace Transactions.Settings;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = null!;
    public string TopicName { get; set; } = null!;
}