using Azure.AI.OpenAI;
using AzureAiMicroservice.Configurations;
using OpenAI.Chat;
using Azure;
using Microsoft.Extensions.Options;

namespace AzureAiMicroservice.Services;

public class AzureAIService(IOptions<AzureAiOptions> options)
{
    private AzureOpenAIClient azureClient =
        new(new Uri(options.Value.Endpoint),
            new AzureKeyCredential(options.Value.Key));

    private static readonly SystemChatMessage SystemMessage = new SystemChatMessage("""
        You are subtask generator model.
        Your task is to take Task title and description, and based on it generate subtasks ( with title and description).
        I need you to provide detailed description for subtasks, including steps to realize the current subtask, also dont forget and specify technical details.
        our response must be  in json format, schema is below.
        Output Format: 
        [ 
            {     
                "title" :  "title",
                "description" : "description" 
            },
        ] 
        FYI. I have a .net core API project with microservice architecture, the communication is organized by REFIT pattern,
         also we have a .net framework MVC application for front part (Angular JS).
        """);

    public async Task<string> GetCompletionAsync(string prompt, CancellationToken ct)
    {
        var chatClient = azureClient.GetChatClient(options.Value.Model);
        var completion = await chatClient.CompleteChatAsync([SystemMessage, new UserChatMessage(prompt)]);

        return completion.Value.Content[0].Text;
    }
}