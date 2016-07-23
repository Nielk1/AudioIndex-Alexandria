using Microsoft.AspNet.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alexandria
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseStreamSystemMiddleware(this IApplicationBuilder builder, StreamSystemMiddlewareOptions options)
        {
            return builder.UseMiddleware<StreamSystemMiddleware>(options);
        }

        public static IApplicationBuilder UseProcessingTimeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ProcessingTimeMiddleware>();
        }
    }
}
