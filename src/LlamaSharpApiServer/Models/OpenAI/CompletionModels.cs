namespace LlamaSharpApiServer.Models.OpenAI;

public class CompletionRequest
{
    public string model { get; set; }
    public string prompt { get; set; }
    public string suffix { get; set; }
    public float temperature { get; set; }
    public int n { get; set; }
    public int max_tokens { get; set; }
    public string stop { get; set; }
    public bool stream { get; set; }
    public float top_p { get; set; }
    public int logprobs { get; set; }
    public bool echo { get; set; }
    public float presence_penalty { get; set; }
    public float frequency_penalty { get; set; }
    public string user { get; set; }
}

public class CompletionResponse
{
    public string id { get; set; }
    public string _object { get; set; }
    public int created { get; set; }
    public string model { get; set; }
    public CompletionResponseChoice[] choices { get; set; }
    public UsageInfo usage { get; set; }
}

public class CompletionResponseChoice
{
    public string text { get; set; }
    public int index { get; set; }
    public object logprobs { get; set; }
    public string finish_reason { get; set; }
}
