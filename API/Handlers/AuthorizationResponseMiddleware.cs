using Microsoft.AspNetCore.Mvc;

namespace API.Handlers;

public class AuthorizationResponseMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Acesso Negado",
                Detail = "Você não tem permissão para acessar este recurso.",
                Status = StatusCodes.Status403Forbidden,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3"
            };
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Não Autenticado",
                Detail = "Você precisa estar autenticado para acessar este recurso.",
                Status = StatusCodes.Status401Unauthorized,
                Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1"
            };
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}