using Domain.Common;

namespace Domain.Aggregates;

public class InvalidAccountNumberException : BadRequestException
{
    public InvalidAccountNumberException() : base("AccountNumber cannot be empty or whitespace.")
    {
    }
}

public class InvalidAccountNumberFormatException : BadRequestException
{
    public InvalidAccountNumberFormatException(string badValue) : base($"AccountNumber with value number {badValue} cannot be created.")
    {
    }
}