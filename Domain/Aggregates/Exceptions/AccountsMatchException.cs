using Domain.Common;

namespace Domain.Aggregates.Exceptions;

public class AccountsMatchException : BadRequestException
{
    public AccountsMatchException(string message, int? code = null) : base(message, code)
    {
    }
}