using Domain.Common;

namespace Domain.Aggregates.Exceptions;

public class InvalidDecreaseException : BadRequestException
{
    public InvalidDecreaseException(string message, int? code = null) : base(message, code)
    {
    }
}