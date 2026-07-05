using System.Diagnostics.CodeAnalysis;

namespace SinistrosApi.Api.Middleware;

public class TratadorDeExcecoes(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { erro = "Ocorreu um erro inesperado." });
        }
    }
}

[ExcludeFromCodeCoverage]
public static class TratadorDeExcecoesExtensions
{
    public static IApplicationBuilder UseTratadorDeExcecoes(this IApplicationBuilder app)
        => app.UseMiddleware<TratadorDeExcecoes>();
}
