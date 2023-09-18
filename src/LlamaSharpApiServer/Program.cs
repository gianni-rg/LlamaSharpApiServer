namespace LlamaSharpApiServer;

using LlamaSharpApiServer.Interfaces;
using LlamaSharpApiServer.Models.OpenAI;
using LlamaSharpApiServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();

        builder.Services.Configure<Models.AppSettings>(
            builder.Configuration.GetSection("Settings")
        );

        builder.Services.AddSingleton<ILLMService, LlamacppService>((services) =>
            new LlamacppService(settings: services.GetRequiredService<IOptions<Models.AppSettings>>().Value
            ));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "AllowCors",
                policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
            );
        });


        var app = builder.Build();

        app.UseCors();

        //if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        var llamaService = app.Services.GetRequiredService<ILLMService>();

        app.MapGet("/", () => "LlamaSharpApiServer (OpenAI-compatible RESTful APIs)");
        app.MapGet("/v1/models", () => llamaService.ShowAvailableModels());
        app.MapPost("/v1/chat/completions", (ChatCompletionRequest request) => llamaService.CreateChatCompletionAsync(request));
        app.MapPost("/v1/completions", () => (CompletionRequest request) => llamaService.CreateCompletion(request));
        app.MapPost("/v1/embeddings", () => (EmbeddingsRequest request) => llamaService.CreateEmbeddings(request));
        app.MapPost("/v1/engines/{modelName}/embeddings", (string modelName, EmbeddingsRequest request) => llamaService.CreateEmbeddings(request));

        var port = app.Services.GetRequiredService<IOptions<Models.AppSettings>>().Value.ServerPort;
        app.Urls.Add($"https://*:{port}");
        app.Run();
    }
}