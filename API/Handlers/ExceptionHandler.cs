using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Handlers {
    public class ExceptionHandler : IExceptionHandler {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {
            var problemDetails = exception switch {
                BadRequestException br => new ProblemDetails {
                    Title = "BadRequest",
                    Detail = br.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"
                },
                ConflictException ce => new ProblemDetails { 
                    Title = "Conflicted",
                    Detail = ce.Message,
                    Status = StatusCodes.Status409Conflict,
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8"
                },
                ResourceNotFoundException rn => new ProblemDetails {
                    Title = "NotFound",
                    Detail = rn.Message,
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4"
                },
                _ => new ProblemDetails {
                    Title = "Server Error",
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                    Status = StatusCodes.Status500InternalServerError,
                }
            };

            httpContext.Response.StatusCode = problemDetails.Status!.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
