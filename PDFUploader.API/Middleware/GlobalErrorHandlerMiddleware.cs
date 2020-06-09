using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PDFUploader.API.Middleware
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlerMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(RequestDelegate));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<GlobalErrorHandlerMiddleware>));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(new ErrorDetails()
                {
                    StatusCode = context.Response.StatusCode,
                    Message = $"Message: {ex.Message}",
                    ErrorType = ex.GetType().ToString()
                }.ToString());

                _logger.LogError(ex, "Unhandled error caught by Global Error Handler");
            }
        }
    }
}