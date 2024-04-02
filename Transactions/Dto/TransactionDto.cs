namespace Transactions.Dto;

public class TransactionDto : BaseDto
{
    public string? Id { get; set; }
    
    public string SenderAccountNumber { get; set; }
    public string RecipientAccountNumber { get; set; }
    
    public decimal Amount { get; set; }
    
    public string Status { get; set; }
}