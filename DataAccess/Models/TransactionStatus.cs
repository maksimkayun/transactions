using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models;

public class TransactionStatus
{
    [Key]
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
}