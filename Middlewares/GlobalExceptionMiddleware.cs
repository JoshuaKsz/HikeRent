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
                ctx.Response.ContentType = "application/json";
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await ctx.Response.WriteAsJsonAsync(new
                {
                    status = "error",
                    // debugging untuk lihat pesan error kalau ingin deploy atau pindah ke branch jangan lupa comment
                    //message = "Terjadi kesalahan server."
                    message = ex.Message
                });
            }
        }
    }
}
