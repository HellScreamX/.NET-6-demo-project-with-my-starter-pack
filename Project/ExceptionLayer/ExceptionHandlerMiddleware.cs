
using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private IExceptionLogger _exceptionLogger;
    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
        _exceptionLogger = null!;
    }

    public async Task Invoke(HttpContext context, IExceptionLogger exceptionLogger)
    {
        _exceptionLogger = exceptionLogger;
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        ExceptionHttpContextInput httpLog = new ExceptionHttpContextInput();
        if (context.User != null) httpLog.userId = context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        httpLog.host = context?.Request?.Host.ToString();
        httpLog.path = context?.Request?.Path.ToString();
        httpLog.method = context?.Request?.Method.ToString();

        var errorCode = "0";
        var statusCode = HttpStatusCode.BadRequest;
        var exceptionType = exception.GetType().Name;
        var response = new Object();

        if (exceptionType == "UnauthorizedAccessException")
        {
            statusCode = HttpStatusCode.Unauthorized;
            //response = new { message = exception.Message };
            response = new { message = "unauthorized" };
        }
        if (exceptionType == "SecurityTokenInvalidSignatureException")
        {
            response = new { message = "Invalid Token" };
        }
        if (exceptionType == "DbUpdateException")
        {
            if(exception?.InnerException?.Message.Contains("Cannot insert duplicate key row in object")??false)
            
            response = new { message = "duplicate key" };
        }
        else if (exceptionType == "CustomException")
        {
            errorCode = ((CustomException)exception).Code;
            response = new { errorCode = errorCode };
        }
        else
        {
            //response = new { message = exception.ToString() };
            response = new { message = "an unhandled error happened" };
        }

        //var response = new { code = statusCode, message = errorCode };       
        var payload = JsonConvert.SerializeObject(response);
        context!.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        //TODO: clear the _context to avoid internal 500 error
        if(exception!=null){
            _exceptionLogger.Log(exception, httpLog, errorCode);
        }
        
        return context.Response.WriteAsync(payload);
    }
}