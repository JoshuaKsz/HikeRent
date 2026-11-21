
using HikeRent.Exceptions;
using System.Net;

namespace HikeRent.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            try
            {
                await _next(ctx);
            }
            catch (Exception ex)
            {
                if (ctx.Response.HasStarted)
                {
                    throw;
                }

                ctx.Response.Clear();
                ctx.Response.ContentType = "application/json";

                var (statusCode, message) = ex switch
                {
                    KeyNotFoundException =>
                        (StatusCodes.Status404NotFound, ex.Message),

                    BusinessException =>
                        (StatusCodes.Status400BadRequest, ex.Message),

                    UnauthorizedAccessException =>
                        (StatusCodes.Status401Unauthorized, "Akses tidak diizinkan."),

                    _ =>
                        (StatusCodes.Status500InternalServerError,
                         "Terjadi kesalahan pada server.")
                };

                ctx.Response.StatusCode = statusCode;

                await ctx.Response.WriteAsJsonAsync(new
                {
                    status = "error",
                    ex.Message
                });
            }
        }
    }
}
