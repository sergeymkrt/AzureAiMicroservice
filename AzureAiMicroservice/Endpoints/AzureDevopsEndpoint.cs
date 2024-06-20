using AzureAiMicroservice.Services;
using FastEndpoints;

namespace AzureAiMicroservice.Endpoints;

public class AzureDevopsEndpoint(AzureDevopsService devopsService, JiraService jiraService) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/api/tasks");
        AllowAnonymous();
    }

    public override Task<object?> ExecuteAsync(CancellationToken ct)
    {
        return base.ExecuteAsync(ct);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await jiraService.GetTasks();
        await SendOkAsync("Azure DevOps endpoint", ct);
    }
}