using Api.Dto;
using Common.Settings;
using Domain.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Transactions.Infrastructure;

public class ExceptionHandleMiddleware(RequestDelegate next, IOptions<AppSettings> options)
{
    public async Task Invoke(HttpContext httpContext, IWebHostEnvironment environment)
    {
        httpContext.Request.EnableBuffering();

        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync<T>(
        HttpContext httpContext,
        T ex) where T : Exception
    {
        var result = new BaseDto
        {
            ErrorInfo = new ErrorInfo
            {
                Message = ex.Message
            }
        };

        var jsonResult = JObject.FromObject(result);
        if(options.Value.Debug)
            jsonResult.Add("stackTrace", ex.StackTrace);
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = ex is BadRequestException or ConflictException ? 400 : 500;

        return httpContext.Response.WriteAsync(jsonResult.ToString());
    }
}