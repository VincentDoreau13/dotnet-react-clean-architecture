using System.Diagnostics;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Domain.Exceptions;

namespace ShopApi.API.Errors;

internal static class CustomErrorHandlerHelper
{
    internal static Task WriteDevelopmentResponse(HttpContext httpContext, Func<Task> next)
    {
        _ = next;

        return WriteResponse(httpContext, true);
    }

    internal static Task WriteProductionResponse(HttpContext httpContext, Func<Task> next)
    {
        _ = next;

        return WriteResponse(httpContext, false);
    }

    private static async Task WriteResponse(HttpContext httpContext, bool includeDetails)
    {
        IExceptionHandlerFeature? exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
        Exception? exception = exceptionDetails?.Error;

        if (exception != null)
        {
            httpContext.Response.ContentType = "application/problem+json";

            var title = "The server encountered an internal error. Please try the request again.";
            string? details = includeDetails ? exception.ToString() : null;
            int status;

            var problem = new ProblemDetails
            {
                Detail = details
            };

            switch (exception)
            {
                case ArgumentNullException:
                    status = StatusCodes.Status400BadRequest;
                    title = exception.Message;
                    break;

                case FunctionalException functionalException:
                    status = StatusCodes.Status400BadRequest;
                    title = exception.Message;
                    problem.Extensions["code"] = functionalException.Code;
                    break;

                case ValidationException validationException:
                    status = StatusCodes.Status400BadRequest;
                    title = "The input pattern is not valid. See the 'validations' table.";
                    problem.Extensions["validations"] = validationException.Errors.Select(error => error.ErrorMessage);
                    break;

                case NotFoundException notFoundException:
                    status = StatusCodes.Status404NotFound;
                    title = exception.Message;
                    problem.Extensions["code"] = notFoundException.Code;
                    break;

                case NotImplementedException:
                    status = StatusCodes.Status501NotImplemented;
                    title = exception.Message;
                    break;

                default:
                    status = StatusCodes.Status500InternalServerError;
                    break;
            }

            problem.Title = title;

            string? traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            if (traceId != null)
                problem.Extensions["traceId"] = traceId;

            httpContext.Response.StatusCode = status;

            Stream stream = httpContext.Response.Body;
            await JsonSerializer.SerializeAsync(stream, problem).ConfigureAwait(false);
        }
    }
}
