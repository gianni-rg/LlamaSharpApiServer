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