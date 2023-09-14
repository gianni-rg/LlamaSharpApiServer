using LlamaSharpApiServer.Models;

namespace LlamaSharpApiServer.Services;

public class LlamacppService
{

    public LlamacppService()
    {
     
    }

    public string ShowAvailableModels()
    {
        return "ShowAvailableModels";
    }

    public string CreateChatCompletion(ChatCompletionRequest request)
    {
        return "CreateChatCompletion";
    }

    public string CreateCompletion(CompletionRequest request)
    {
        return "CreateCompletion";
    }

    public string CreateEmbeddings(EmbeddingsRequest request)
    {
        return "CreateEmbeddings";
    }
}