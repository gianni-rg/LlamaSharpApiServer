namespace LlamaSharpApiServer.Models.Internal;

public class GenerationParameters
{
    public string Model { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public float Temperature { get; set; }
    public float TopP { get; set; }
    public int MaxNewTokens { get; set; }
    public bool Echo { get; set; }
    public List<string> Stop { get; set; } = new List<string>();
    public List<int> StopTokenIds { get; set; } = new List<int>();

}