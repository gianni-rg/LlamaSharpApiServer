namespace LlamaSharpApiServer;

using LlamaSharpApiServer.Models;
using LlamaSharpApiServer.Services;

/// <summary>
/// A server that provides OpenAI-compatible RESTful APIs. It supports:<br/>
/// - Chat Completions (Reference: https://platform.openai.com/docs/api-reference/chat)<br/>
/// - Completions (Reference: https://platform.openai.com/docs/api-reference/completions)<br/>
/// - Embeddings (Reference: https://platform.openai.com/docs/api-reference/embeddings)<br/>
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var llamaService = new LlamacppService();

        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/", () => "LlamaSharpApiServer (OpenAI-compatible RESTful APIs)");
        app.MapGet("/v1/models", () => llamaService.ShowAvailableModels());
        app.MapPost("/v1/chat/completions", (ChatCompletionRequest request) => llamaService.CreateChatCompletion(request));
        app.MapPost("/v1/completions", () => (CompletionRequest request) => llamaService.CreateCompletion(request));
        app.MapPost("/v1/embeddings", () => (EmbeddingsRequest request) => llamaService.CreateEmbeddings(request));
        app.MapPost("/v1/engines/{modelName}/embeddings", (string modelName, EmbeddingsRequest request) => llamaService.CreateEmbeddings(request));

        app.Run();
    }
}