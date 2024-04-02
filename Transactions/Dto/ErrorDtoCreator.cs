namespace Transactions.Dto;

public static class ErrorDtoCreator
{
    public static T Create<T>(string message) where T : BaseDto?, new()
        => new T
        {
            ErrorInfo = new ErrorInfo()
            {
                Message = message
            }
        };
}