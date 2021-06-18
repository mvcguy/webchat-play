using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebChatPlay
{
    public static class XHeaderExtention
    {
        public static IApplicationBuilder UseXHeaders(this IApplicationBuilder builder)
        {
            builder.Use(UseXHeaders);
            return builder;
        }

        private static Task UseXHeaders(HttpContext context, Func<Task> next)
        {
            context.Response.Headers.Add("X-ServerDateTime", DateTime.Now.ToString("u"));
            return next();
        }
    }
}
