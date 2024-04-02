using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Account
{
    [Key]
    [Column("id")]
    public string? Id { get; set; }
    
    public long AccountNumber { get; set; }
    
    public Customer Owner { get; set; }
    
    public List<Transaction> OutgoingTransactions { get; set; }
    public List<Transaction> IncomingTransactions { get; set; }
    
    public decimal Amount { get; set; }
}