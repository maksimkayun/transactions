using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Customer
{
    [Key]
    [Column("id")]
    public string? Id { get; set; }

    public string Name { get; set; }

    public virtual List<Account> Accounts { get; set; }
}