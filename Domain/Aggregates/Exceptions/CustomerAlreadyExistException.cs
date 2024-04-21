using Domain.Common;

namespace Domain.Aggregates.Exceptions;

public class CustomerAlreadyExistException: ConflictException
{
    public CustomerAlreadyExistException(int? code = default) : base("Customer already exist!", code)
    {
    }
}