namespace Transactions.Dto;

public class AccountDto : BaseDto
{
    public string AccountNumber { get; set; }
    public string OwnerId { get; set; }
    public decimal Amount { get; set; }
    public List<string>? OutgoingTransactionIds { get; set; }
    public List<string>? IncomingTransactionIds { get; set; }
}