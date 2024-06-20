using AzureAiMicroservice.Models;
using FastEndpoints;
using FluentValidation.Results;

namespace AzureAiMicroservice.PreProcessors;

public class SecretKeyAuthPreProcessor : IPreProcessor<MessageModel>
{
    private readonly string _secretKey;

    public SecretKeyAuthPreProcessor(IConfiguration configuration)
    {
        _secretKey = configuration["SecretKey"];
    }
    
    public async Task PreProcessAsync(IPreProcessorContext<MessageModel> context, CancellationToken ct)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var extractedSecretKey))
        {
            context.HttpContext.Response.StatusCode = 401;
            context.ValidationFailures.Add(new ValidationFailure("Missing Authorization Header", "The Authorization header needs to be set!"));

            await context.HttpContext.Response.SendErrorsAsync(context.ValidationFailures, cancellation: ct);
        }
        if (!_secretKey.Equals(extractedSecretKey))
        {
            context.HttpContext.Response.StatusCode = 403;
            context.ValidationFailures.Add(new ValidationFailure("Unauthorized", "Unauthorized client."));
            await context.HttpContext.Response.SendErrorsAsync(context.ValidationFailures, cancellation: ct);
        }
    }
}