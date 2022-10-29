using static BookWebAPI.Common.CustomExceptions;

namespace BookWebAPI.Extensions
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context,Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizeException => StatusCodes.Status401Unauthorized,
                ExistsException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            var errorDetails = new ErrorDetails()
            {
                Error = context.Response.StatusCode,
                Message = exception.Message
            };


            await context.Response.WriteAsync(errorDetails.ToString());
        }
    }
}
