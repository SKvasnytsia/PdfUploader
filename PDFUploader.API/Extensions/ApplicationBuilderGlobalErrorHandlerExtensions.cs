using Microsoft.AspNetCore.Builder;
using PDFUploader.API.Middleware;

namespace PDFUploader.API.Extensions
{
    public static class ApplicationBuilderGlobalErrorHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalErrorHandlerMiddleware>();
        }
    }
}
