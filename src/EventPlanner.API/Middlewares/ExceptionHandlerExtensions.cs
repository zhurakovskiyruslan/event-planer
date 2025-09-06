using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using EventPlanner.Application.Common.Exceptions;

namespace EventPlanner.API.Middlewares;

public static class ExceptionHandlerExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                var ex = feature?.Error;

                var status = ex switch
                {
                    ValidationException => StatusCodes.Status400BadRequest,
                    NotFoundException   => StatusCodes.Status404NotFound,
                    ConflictException   => StatusCodes.Status409Conflict,
                    ForbiddenException  => StatusCodes.Status403Forbidden,
                    _                   => StatusCodes.Status500InternalServerError
                };

                context.Response.StatusCode = status;
                context.Response.ContentType = "application/problem+json";

                var problem = new
                {
                    type = "about:blank",
                    title = ex?.Message ?? "Unhandled error",
                    status,
                    traceId = context.TraceIdentifier
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            });
        });
    }
}