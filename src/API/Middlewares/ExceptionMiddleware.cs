using Application.Shared.Helpers.Responses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        // ✅ FluentValidation
        catch (ValidationException ex)
        {
            await Write(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await Write(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await Write(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await Write(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (DbUpdateException ex)
        {
            await Write(context, HttpStatusCode.BadRequest, "Məlumat bazası xətası baş verdi.");
        }
        catch (Exception)
        {
            await Write(context, HttpStatusCode.InternalServerError, "Gözlənilməz xəta baş verdi.");
        }
    }

    private static async Task Write(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(BaseResponse.Fail(message));
    }
}
