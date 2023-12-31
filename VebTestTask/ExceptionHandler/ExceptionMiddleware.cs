﻿using System.Net;
using System.Net.Mime;
using System.Text.Json;
using VebTestTask.Wrapper;

namespace VebTestTask.ExceptionHandler;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Creates InternalServerError response based on catched exception
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ex"></param>
    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = _env.IsDevelopment()
            ? new Response<string> { Succeeded = false, Errors = new[] { ex.StackTrace }!, Message = ex.Message }
            : new Response<string> { Succeeded = false, Message = "Internal Server Error" };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);
        
        await context.Response.WriteAsync(json);
    }
}