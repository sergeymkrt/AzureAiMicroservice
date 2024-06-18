using AzureAiMicroservice.Models.Requests;
using AzureAiMicroservice.Models.Responses;
using FastEndpoints;

namespace AzureAiMicroservice.Endpoints;

public class HelloEndpoint : Endpoint<HelloRequest, HelloResponse>
{
    public override void Configure()
    {
        Post("/api/hello");
        AllowAnonymous();
    }

    public override async Task HandleAsync(HelloRequest req, CancellationToken ct)
    {
        var message = $"Hello, {req.Name}!";
        await SendOkAsync(new HelloResponse { Message = message }, ct);
    }
}