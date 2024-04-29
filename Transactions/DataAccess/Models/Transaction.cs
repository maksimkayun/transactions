using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transactions.DataAccess.Models;

public class Transaction
{
    [Key]
    [Column("id")]
    public string? Id { get; set; }
    
    public Account SenderAccount { get; set; }
    public Account RecipientAccount { get; set; }
    
    public decimal Amount { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
}