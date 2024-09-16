using Transactions.Settings;

namespace Common.Settings;

public class AppSettings
{
    public string ConnectionString { get; set; } = null!;
    public bool Debug { get; set; } = false;

    public KafkaSettings KafkaSettings { get; set; } = null!;
    
    public bool UseSelfTransactionProcessor { get; set; }
}