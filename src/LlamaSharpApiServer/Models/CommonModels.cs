namespace LlamaSharpApiServer.Models;

public class UsageInfo
{
    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
}
