
public static class Extensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        => builder.UseMiddleware(typeof(ExceptionHandlerMiddleware));
}