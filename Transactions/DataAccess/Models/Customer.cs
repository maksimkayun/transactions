using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transactions.DataAccess.Models;

public class Customer
{
    [Key]
    [Column("id")]
    public string? Id { get; set; }

    public string Name { get; set; }

    public virtual List<Account> Accounts { get; set; }
}