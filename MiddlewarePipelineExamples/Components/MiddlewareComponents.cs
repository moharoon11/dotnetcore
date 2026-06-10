namespace MiddlewarePipelineExamples.Components
{
    public class MiddlewareComponents
    {



        public static async Task Component1(HttpContext context, RequestDelegate next)
        {
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync("1. Middleware Component Started...\n");

            await next(context);

            await context.Response.WriteAsync("1. Middleware Component Finished...\n");

        }

        public static async Task Component2(HttpContext context, RequestDelegate next)
        {
            await context.Response.WriteAsync("2. Middleware Component Started...\n");
            await next(context);

            await context.Response.WriteAsync("2. Middleware Component Finished...\n");
        }

        public static async Task Component3(HttpContext context, RequestDelegate next)
        {
            await context.Response.WriteAsync("3. Middleware Component Started...\n");

            await next(context);

            await context.Response.WriteAsync("3. Middleware component Finished...\n");
        }

        public static void StartNewPipelineBranch(IApplicationBuilder builder)
        {
            builder.Use(Component4);
            builder.Use(Component5);
        }

        public static async Task Component4(HttpContext context, RequestDelegate next)
        {
            await context.Response.WriteAsync("4. Middleware Component Started...\n");

            await next(context);

            await context.Response.WriteAsync("4. Middleware component Finished...\n");
        }

        public static async Task Component5(HttpContext context, RequestDelegate next)
        {
            await context.Response.WriteAsync("5. Middleware Component Started...\n");

            await next(context);

            await context.Response.WriteAsync("5. Middleware component Finished...\n");
        }

        public static bool CheckPathForNewPipeline(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/new-branch"))
            {
                return true;
            }

            return false;
        }

        public static void StartNewBranchPipeLineWithCondition(IApplicationBuilder builder)
        {
            builder.Use(Component6);
            builder.Use(Component7);
        }

    
        public static async Task Component6(HttpContext context, RequestDelegate next)
        {
            await context.Response.WriteAsync("6. Middleware Component Started...\n");

            await next(context);

            await context.Response.WriteAsync("6. Middleware component Finished...\n");
        }

        public static async Task Component7(HttpContext context, RequestDelegate next)
        {
            await context.Response.WriteAsync("7. Middleware Component Started...\n");

            await next(context);

            await context.Response.WriteAsync("7. Middleware component Finished...\n");
        }

        
        public static void UseEndpointsComponent(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/get_app_details", async (HttpContext context) =>
            {
                await context.Response.WriteAsync("This endpoint returns application details...");
                throw new Exception("Simulated exception in endpoint...");
            });
        }



    }
}