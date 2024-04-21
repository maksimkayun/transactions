using Api.Dto;
using Api.Dto.CreateDto;
using DataAccess;
using DataAccess.Models;
using Domain.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services.Implementations;

public class CustomerService : ICustomerService
{
    private TransactionsContext context;
    private readonly AccountNumberHelper _accountNumberHelper;

    public CustomerService(TransactionsContext context)
    {
        this.context = context;
        _accountNumberHelper = new AccountNumberHelper();
        var maxNum = context.Accounts.Any() ? context.Accounts.Max(e => e.AccountNumber) : -1;
        _accountNumberHelper.SetStartNumber(maxNum);
    }

    public async Task<CustomerDto?> RegisterCustomer(CreateCustomerDto customerDto, CancellationToken cancellationToken)
    {
        var custDb = await context.Customers.FirstOrDefaultAsync(e => e.Name == customerDto.Name, cancellationToken);
        if (custDb != default)
        {
            return ErrorDtoCreator.Create<CustomerDto?>("Клиент с указанным именем уже зарегистрирован");
        }

        var newCust = new Aggregates.Customer(name: customerDto.Name);
        custDb = new Customer
        {
            Id = newCust.CustomerId,
            Name = newCust.Name,
            Accounts = new List<Account>()
        };
        await context.Customers.AddAsync(custDb, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Mapper.MapToDto(custDb);
    }

    public async Task<CustomerDto?> FindCustomer(string customerId, CancellationToken cancellationToken)
    {
        var custDb = await context.Customers
            .Include(e=>e.Accounts)
            .ThenInclude(e => e.OutgoingTransactions)
            .Include(e=>e.Accounts)
            .ThenInclude(e=>e.IncomingTransactions)
            .FirstOrDefaultAsync(e => e.Id == customerId, cancellationToken);
        if (custDb == default)
        {
            return ErrorDtoCreator.Create<CustomerDto?>("Клиент не найден");
        }

        return Mapper.MapToDto(custDb);
    }

    public async Task<AccountDto?> OpenAccount(string customerId, CancellationToken cancellationToken)
    {
        var customerDb = await context.Customers.FirstOrDefaultAsync(e => e.Id == customerId, cancellationToken);
        if (customerDb == null)
        {
            return ErrorDtoCreator.Create<AccountDto?>($"Клиент с ID={customerId} не найден");
        }
        
        var customer = new Aggregates.Customer(customerDb.Name, customerId);
        
        var acc = customer.OpenAccount(_accountNumberHelper.GetNumber());
        var accDb = new Account
        {
            Id = acc.Id,
            AccountNumber = long.Parse(acc.Number),
            Owner = customerDb,
            OutgoingTransactions = [],
            IncomingTransactions = []
        };

        var savedAccDb = (await context.Accounts.AddAsync(accDb, cancellationToken)).Entity;
        await context.SaveChangesAsync(cancellationToken);
        return new AccountDto
        {
            AccountNumber = accDb.AccountNumber.ToString(),
            OwnerId = savedAccDb.Owner.Id,
            Amount = savedAccDb.Amount,
            OutgoingTransactionIds = [],
            IncomingTransactionIds = []
        };
    }

    public async Task<List<CustomerDto>> GetCustomers(int take, int skip, CancellationToken cancellationToken)
        => context.Customers
            .Skip(skip).Take(take)
            .AsEnumerable()
            .Select(Mapper.MapToDto)
            .ToList();
    
    internal class AccountNumberHelper
    {
        private long _number = 77000000;
        private readonly List<long> UsedIds = [];
        private readonly Stack<long> ResurrectedIds = [];

        public long GetNumber()
        {
            if (ResurrectedIds.TryPop(out var num))
            {
                return num;
            }
            UsedIds.Add(_number++);
            return _number;
        }

        public void ResurrectNumber(string number)
        {
            if (long.TryParse(number, out var num) )
            {
                UsedIds.RemoveAll(e => e == num);
                ResurrectedIds.Push(num);
            }
        }

        public void SetStartNumber(long number)
        {
            if (number > 0)
            {
                _number = number;
            }
        }
    }
}