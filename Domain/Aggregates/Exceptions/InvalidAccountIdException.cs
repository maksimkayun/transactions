using Domain.Common;

namespace Domain.Aggregates.Exceptions;

public class InvalidAccountIdException : BadRequestException
{
    public InvalidAccountIdException(Guid accountId)
        : base($"accountId: '{accountId}' is invalid.")
    {
    }
}