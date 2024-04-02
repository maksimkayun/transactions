namespace Transactions.Dto;

public class CustomerDto : BaseDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<AccountDto>? Accounts { get; set; }
}