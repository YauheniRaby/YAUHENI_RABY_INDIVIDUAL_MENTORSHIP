﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WeatherApi.Infrastructure
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Error: {ex}");
                httpContext.Response.StatusCode = StatusCodes.Status408RequestTimeout;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}