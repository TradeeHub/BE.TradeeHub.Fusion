using BE.TradeeHub.Fusion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CookiePropagatingHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("GraphQLCorsPolicy", builder =>
    {
        builder.WithOrigins(["http://localhost:3000","http://localhost:5020","http://172.17.0.1:5020"])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddHttpClient("Fusion").AddHttpMessageHandler<CookiePropagatingHandler>();

builder.Services
    .AddFusionGatewayServer()
    .ConfigureFromFile("./gateway.fgp");

var app = builder.Build();

app.UseCors("GraphQLCorsPolicy"); // Apply the CORS policy
app.UseRouting();
app.MapGraphQL();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/graphql/", permanent: false);
    }
    else
    {
        await next();
    }
});

app.RunWithGraphQLCommands(args);