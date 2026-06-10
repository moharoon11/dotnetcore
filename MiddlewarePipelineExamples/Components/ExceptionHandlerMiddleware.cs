using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MiddlewarePipelineExamples.Components
{

    public class ExceptionHandlerMiddleware : IMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch(Exception ex)
            {
                string ExceptionTemplate = "<!DOCTYPE html><html><head><title>Exception Occurred</title></head><body><h1>An exception occurred while processing your request.</h1><p>{0}</p></body></html>";
                string responseContent = string.Format(ExceptionTemplate, ex.Message);
               
                await context.Response.WriteAsync(responseContent);
            }
        }

    }
}