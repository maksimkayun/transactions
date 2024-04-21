using Api.Dto;
using Api.Dto.CreateDto;

namespace Domain.Services;

public interface ICustomerService
{
    public Task<CustomerDto?> RegisterCustomer(CreateCustomerDto customerDto, CancellationToken cancellationToken);
    public Task<CustomerDto?> FindCustomer(string customerId, CancellationToken cancellationToken);
    public Task<AccountDto?> OpenAccount(string customerId, CancellationToken cancellationToken);
    
    
    public Task<List<CustomerDto>> GetCustomers(int take, int skip, CancellationToken cancellationToken);
}