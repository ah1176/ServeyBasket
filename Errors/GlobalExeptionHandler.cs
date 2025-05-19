﻿using Microsoft.AspNetCore.Diagnostics;

namespace Survey_Basket.Errors
{
    public class GlobalExeptionHandler(ILogger<GlobalExeptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExeptionHandler> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception,"some thing went wrong : {Messeage}", exception.Message);

            var problemDetails = new ProblemDetails 
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.2"
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(problemDetails,cancellationToken);

            return true;
        }
    }
}
