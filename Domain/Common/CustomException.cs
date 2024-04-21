using System.Net;

namespace Domain.Common;

public class BadRequestException : CustomException
{
    public BadRequestException(string message, int? code = null) : base(message, code: code)
    {

    }
}

public class ConflictException : CustomException
{
    public ConflictException(string message, int? code = null) : base(message, code: code)
    {
    }
}

public class CustomException : System.Exception
{
    public CustomException(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        int? code = null) : base(message)
    {
        StatusCode = statusCode;
        Code = code;
    }

    public CustomException(
        string message,
        System.Exception innerException,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        int? code = null) : base(message, innerException)
    {
        StatusCode = statusCode;
        Code = code;
    }

    public CustomException(
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        int? code = null) : base()
    {
        StatusCode = statusCode;
        Code = code;
    }

    public HttpStatusCode StatusCode { get; }

    public int? Code { get; }
}