using Domain.Common;

namespace Domain.Aggregates.Exceptions;

public class InvalidCustomerIdException: BadRequestException
{
    public InvalidCustomerIdException(Guid customerId)
        : base($"customerId: '{customerId}' is invalid.")
    {
    }
    
    public InvalidCustomerIdException(string customerId)
        : base($"customerId: '{customerId}' is invalid.")
    {
    }
}