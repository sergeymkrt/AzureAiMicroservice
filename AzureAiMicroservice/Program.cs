using AzureAiMicroservice.Configurations;
using AzureAiMicroservice.Services;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument()
    .AddHttpClient();

builder.Services.Configure<AzureAiOptions>(builder.Configuration.GetSection("OpenAI"));
builder.Services.Configure<AzureDevopsOptions>(builder.Configuration.GetSection("Devops"));
builder.Services.Configure<JiraOptions>(builder.Configuration.GetSection("Jira"));
builder.Services.AddSingleton<AzureAIService>();

builder.Services.AddScoped<AzureDevopsService>();
builder.Services.AddScoped<JiraService>();

var app = builder.Build();

app.UseHttpsRedirection();

app
   .UseFastEndpoints(c =>
   {
       c.Endpoints.RoutePrefix = "api";
   })
   .UseSwaggerGen();

app.Run();
