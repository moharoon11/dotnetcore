using MiddlewarePipelineExamples.Components;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(MiddlewareComponents.Component1);
app.Use(MiddlewareComponents.Component2);

app.Map("/pipeline", MiddlewareComponents.StartNewPipelineBranch);
app.MapWhen(MiddlewareComponents.CheckPathForNewPipeline, MiddlewareComponents.StartNewBranchPipeLineWithCondition);

app.UseWhen(
    (context) => context.Request.Path.ToString().Equals("/rejoin_pipleline"), 
    (builder) =>
    {
        builder.Use(MiddlewareComponents.Component6);
        builder.Use(MiddlewareComponents.Component7);
    }
);

app.Use(MiddlewareComponents.Component3);

app.Run();
