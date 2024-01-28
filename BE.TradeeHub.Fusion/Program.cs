using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using BE.TradeeHub.Fusion;

var builder = WebApplication.CreateBuilder(args);
var appSettings = new AppSettings(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton(appSettings);
builder.Services.AddTransient<CookiePropagatingHandler>();
builder.Services.AddSingleton<AuthService>();

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

// If I am on dev the setting should come from bottom right of rider aws toolkit no need to pass any values
if (appSettings.Environment.Contains("dev", StringComparison.CurrentCultureIgnoreCase))
{
    builder.Services.AddScoped<IAmazonCognitoIdentityProvider, AmazonCognitoIdentityProviderClient>();
}
else
{
    //this means I am in docker and values are not saved anywhere in the solution but only in my docker environment variable you can edit the docker in rider ide(not the file)
    var awsOptions = builder.Configuration.GetAWSOptions();

    awsOptions.Credentials = new BasicAWSCredentials(
        appSettings.AwsAccessKeyId, 
        appSettings.AwsSecretAccessKey
    );
    builder.Services.AddSingleton<IAmazonCognitoIdentityProvider>(sp =>
        new AmazonCognitoIdentityProviderClient(awsOptions.Credentials, appSettings.AWSRegion)
    );
}

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