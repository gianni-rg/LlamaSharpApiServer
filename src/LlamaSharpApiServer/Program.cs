// Copyright (C) 2023 Gianni Rosa Gallina. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License").
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace LlamaSharpApiServer;

using LlamaSharpApiServer.Interfaces;
using LlamaSharpApiServer.Models.OpenAI;
using LlamaSharpApiServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text.Json;

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

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        var llamaService = app.Services.GetRequiredService<ILLMService>();

        app.MapGet("/", () => "LlamaSharpApiServer (OpenAI-compatible RESTful APIs)");
        app.MapGet("/v1/models", () => llamaService.ShowAvailableModels());

        app.MapPost("/v1/chat/completions", async Task<IResult> (ChatCompletionRequest request, HttpContext http) =>
        {
            try
            {
                // DEBUG
                request.max_tokens = 256;

                //Console.WriteLine(JsonSerializer.Serialize(request));

                if (request.stream)
                {
                    http.Response.Headers.CacheControl = "no-cache";
                    http.Response.Headers.ContentType = "text/event-stream";
                    await http.Response.Body.FlushAsync();
                    await foreach (var content in llamaService.CreateChatCompletionStream(request))
                    {
                        await http.Response.WriteAsync(content);
                        await http.Response.Body.FlushAsync();
                    }
                    return Results.Empty;
                }
                else
                {
                    return Results.Ok(await llamaService.CreateChatCompletionAsync(request));
                }
            }
            catch (Exception ex)
            {
                return Results.Problem($"{ex}");
            }
        });

        app.MapPost("/v1/completions", async Task<IResult> (CompletionRequest request, HttpContext http) =>
        {
            try
            {
                // DEBUG
                Console.WriteLine(JsonSerializer.Serialize(request));

                if (request.stream)
                {
                    http.Response.Headers.CacheControl = "no-cache";
                    http.Response.Headers.ContentType = "text/event-stream";
                    await http.Response.Body.FlushAsync();
                    await foreach (var content in llamaService.CreateCompletionStream(request))
                    {
                        await http.Response.WriteAsync(content);
                        await http.Response.Body.FlushAsync();
                    }
                    return Results.Empty;
                }
                else
                {
                    return Results.Ok(await llamaService.CreateCompletionAsync(request));
                }
            }
            catch (Exception ex)
            {
                return Results.Problem($"{ex}");
            }
        });

        app.MapPost("/v1/embeddings", (EmbeddingsRequest request) => llamaService.CreateEmbeddings(request));
        app.MapPost("/v1/{modelName}/embeddings", (string modelName, EmbeddingsRequest request) => llamaService.CreateEmbeddings(request));

        var port = app.Services.GetRequiredService<IOptions<Models.AppSettings>>().Value.ServerPort;

        // Serve the application on HTTP port, otherwise Continue.dev will fail with certificate error
        app.Urls.Add($"http://*:{port}");

        app.Run();
    }
}