// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace LlamaSharpApiServer.Interfaces;

using LlamaSharpApiServer.Models.OpenAI;

public interface ILLMService
{
    ModelsResponse ShowAvailableModels();

    Task<ChatCompletionResponse> CreateChatCompletionAsync(ChatCompletionRequest request);

    IAsyncEnumerable<string> CreateChatCompletionStream(ChatCompletionRequest request);

    Task<CompletionResponse> CreateCompletionAsync(CompletionRequest request);
    IAsyncEnumerable<string> CreateCompletionStream(CompletionRequest request);

    EmbeddingsResponse CreateEmbeddings(EmbeddingsRequest request);
}