using Domain.Common;

namespace Domain.Aggregates.Exceptions;

public class AccountDoesNotExistException : ConflictException
{
    public AccountDoesNotExistException(long? number = default) : base($"Account with number={number} does not exists")
    {
    }
}