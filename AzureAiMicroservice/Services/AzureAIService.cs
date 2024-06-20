using Azure.AI.OpenAI;
using AzureAiMicroservice.Configurations;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using Microsoft.Extensions.Options;

namespace AzureAiMicroservice.Services;

public class AzureAIService(IOptions<AzureAiOptions> options)
{
    private readonly OpenAIClient _client = new(
        new ApiKeyCredential(options.Value.Key),
        new AzureOpenAIClientOptions
        {
            Endpoint = new Uri(options.Value.Endpoint)
        });

    public async Task<string> GetCompletionAsync(string prompt, CancellationToken ct)
    {
        var chatCompletionsOptions = new ChatCompletionOptions
        {
            Temperature = (float)0.5,
            MaxTokens = 800,
            FrequencyPenalty = 0,
            PresencePenalty = 0
        };

        var response = await _client
            .GetChatClient(options.Value.Model)
            .CompleteChatAsync([new AssistantChatMessage(prompt)], chatCompletionsOptions, ct);

        return response.Value.Content[0].Text;
    }
}