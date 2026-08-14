using MarketSphere.Api.Contracts;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.ExceptionServices;

namespace MarketSphere.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await WriteErrorAsync(
                context,
                exception);
        }
    }

    private async Task WriteErrorAsync(
        HttpContext context,
        Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var errorCode = "internal_error";
        var message = "An unexpected error occurred.";
        IReadOnlyDictionary<string, string[]>? errors = null;

        switch (exception)
        {
            case AppValidationException validation:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = "validation_error";
                message = validation.Message;
                errors = validation.Errors;
                break;

            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorCode = "not_found";
                message = exception.Message;
                break;

            case ForbiddenBusinessActionException:
                statusCode = HttpStatusCode.Forbidden;
                errorCode = "forbidden";
                message = exception.Message;
                break;

            case ConflictException:
            case DbUpdateConcurrencyException:
                statusCode = HttpStatusCode.Conflict;
                errorCode = "conflict";
                message = exception.Message;
                break;

            case BusinessRuleException:
                statusCode = HttpStatusCode.UnprocessableEntity;
                errorCode = "business_rule_error";
                message = exception.Message;
                break;

            case DbUpdateException:
                statusCode = HttpStatusCode.Conflict;
                errorCode = "database_constraint_error";
                message = "The operation violated a database constraint.";
                break;
        }

        if ((int)statusCode >= 500)
        {
            _logger.LogError(
                exception,
                "Unhandled exception. TraceID: {TraceID}",
                context.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Request failed with {StatusCode}. TraceID: {TraceID}",
                (int)statusCode,
                context.TraceIdentifier);
        }

        if (context.Response.HasStarted)
        {
            ExceptionDispatchInfo
                .Capture(exception)
                .Throw();
        }

        context.Response.Clear();
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse(
            false,
            message,
            errorCode,
            context.TraceIdentifier,
            errors);

        await context.Response.WriteAsJsonAsync(response);
    }
}
