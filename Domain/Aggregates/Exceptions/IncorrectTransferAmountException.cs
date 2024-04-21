using Domain.Common;

namespace Domain.Aggregates.Exceptions;

public class IncorrectTransferAmountException : BadRequestException
{
    public IncorrectTransferAmountException(string message, int? code = null) : base(message, code)
    {
    }
}