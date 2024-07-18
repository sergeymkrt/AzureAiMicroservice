using AzureAiMicroservice.Services;
using FastEndpoints;

namespace AzureAiMicroservice.Endpoints;

public class JiraEndpoint(JiraService jiraService) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("tasks");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(CancellationToken ct)
    {
        var stream = await jiraService.GetTasks(50);
        await SendStreamAsync(stream, "trainData.json", cancellation: ct);
    }
}

