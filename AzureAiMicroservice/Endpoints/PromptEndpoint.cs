using AzureAiMicroservice.Models;
using AzureAiMicroservice.PreProcessors;
using AzureAiMicroservice.Services;
using FastEndpoints;

namespace AzureAiMicroservice.Endpoints;

public class PromptEndpoint(AzureAIService azureAiService) : Endpoint<MessageModel, MessageModel>
{
    public override void Configure()
    {
        Post("/api/prompt");
        PreProcessor<SecretKeyAuthPreProcessor>();
    }
    
    public override async Task HandleAsync(MessageModel req, CancellationToken ct)
    {
        var response = await azureAiService.GetCompletionAsync(req.Message, ct);
        await SendOkAsync(new MessageModel { Message = response }, ct);
    }
}