using AzureAiMicroservice.DTOs;
using AzureAiMicroservice.Models;
using AzureAiMicroservice.PreProcessors;
using AzureAiMicroservice.Services;
using FastEndpoints;

namespace AzureAiMicroservice.Endpoints;

public class PromptEndpoint(AzureAIService azureAiService) : Endpoint<PromptDto, MessageModel>
{
    public override void Configure()
    {
        Post("prompt");
        PreProcessor<SecretKeyAuthPreProcessor<PromptDto>>();
    }

    public override async Task HandleAsync(PromptDto req, CancellationToken ct)
    {
        var prompt = $"Title: {req.Title}\nDescription: {req.Description}";
        var response = await azureAiService.GetCompletionAsync(prompt, ct);
        await SendOkAsync(new MessageModel { Message = response }, ct);
    }
}