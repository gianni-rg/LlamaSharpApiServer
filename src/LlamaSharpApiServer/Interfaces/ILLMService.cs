namespace LlamaSharpApiServer.Interfaces;

using LlamaSharpApiServer.Models.OpenAI;

public interface ILLMService
{
    ModelsResponse ShowAvailableModels();

    Task<ChatCompletionResponse> CreateChatCompletionAsync(ChatCompletionRequest request);

    CompletionResponse CreateCompletion(CompletionRequest request);

    EmbeddingsResponse CreateEmbeddings(EmbeddingsRequest request);
}