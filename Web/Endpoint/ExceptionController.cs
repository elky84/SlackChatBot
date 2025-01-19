using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace Web.Endpoint
{
    public static class ExceptionController
    {
        public static object Handler(HttpContext context)
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = feature?.Error;
            if (exception is null)
                return new
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Unhandled exception"
                };

            return exception switch
            {
                _ => new { StatusCode = HttpStatusCode.InternalServerError, Message = exception.Message }
            };
        }
    }
}
