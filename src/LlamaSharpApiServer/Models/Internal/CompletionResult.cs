namespace LlamaSharpApiServer.Models.Internal;

/// <summary>
/// Represents a completion result from the model
/// </summary>
public class CompletionResult
{
    public string Text { get; set; } = string.Empty;
    public int? LogProbs { get; set; } = null;
    public UsageInformation Usage { get; set; } = new UsageInformation();
    public string? FinishReason { get; set; } = null;
    public int ErrorCode { get; set; } = 0;
}
