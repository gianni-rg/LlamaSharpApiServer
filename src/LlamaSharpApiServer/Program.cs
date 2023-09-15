namespace LlamaSharpApiServer;

using LlamaSharpApiServer.Models.OpenAI;
using LlamaSharpApiServer.Services;
using Microsoft.AspNetCore.Builder;

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
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        //app.UseCors();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapGet("/", () => "LlamaSharpApiServer (OpenAI-compatible RESTful APIs)");
        app.MapGet("/v1/models", () => llamaService.ShowAvailableModels());
        app.MapPost("/v1/chat/completions", (ChatCompletionRequest request) => llamaService.CreateChatCompletionAsync(request));
        app.MapPost("/v1/completions", () => (CompletionRequest request) => llamaService.CreateCompletion(request));
        app.MapPost("/v1/embeddings", () => (EmbeddingsRequest request) => llamaService.CreateEmbeddings(request));
        app.MapPost("/v1/engines/{modelName}/embeddings", (string modelName, EmbeddingsRequest request) => llamaService.CreateEmbeddings(request));

        app.Run();
    }
}