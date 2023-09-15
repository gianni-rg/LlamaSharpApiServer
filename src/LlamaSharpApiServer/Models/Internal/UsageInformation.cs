namespace LlamaSharpApiServer.Models.Internal;

public class UsageInformation
{
    public int PromptTokens { get; set; } = 0;
    public int CompletionTokens { get; set; } = 0;
    public int TotalTokens { get; set; } = 0;
}