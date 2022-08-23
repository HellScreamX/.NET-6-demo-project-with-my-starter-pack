using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

[AttributeUsage(AttributeTargets.Method)]
public class ThrottleAttribute : ActionFilterAttribute
{
    public string Name { get; set; }="main";
    public int Seconds { get; set; }=1;
    public string Message { get; set; }="try again in {n} seconds";

    private static MemoryCache Cache { get; } = new MemoryCache(new MemoryCacheOptions());

    public override void OnActionExecuting(ActionExecutingContext c)
    {
        var key = string.Concat(Name, "-", c.HttpContext.Request.HttpContext.Connection.RemoteIpAddress);

        if (!Cache.TryGetValue(key, out bool entry))
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(Seconds));

            Cache.Set(key, true, cacheEntryOptions);
        }
        else 
        {           
            c.Result = new ContentResult {Content = Message.Replace("{n}", Seconds.ToString())};
            c.HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
        }
    }
}