using Transactions.Aggregates;
using Transactions.Dto;
using Transactions.Dto.CreateDto;

namespace Transactions.DomainServices;

public interface ICustomerService
{
    public Task<CustomerDto?> RegisterCustomer(CreateCustomerDto customerDto, CancellationToken cancellationToken);
    public Task<CustomerDto?> FindCustomer(string customerId, CancellationToken cancellationToken);
    public Task<AccountDto?> OpenAccount(string customerId, CancellationToken cancellationToken);
    
    
    public Task<List<CustomerDto>> GetCustomers(int take, int skip, CancellationToken cancellationToken);
}