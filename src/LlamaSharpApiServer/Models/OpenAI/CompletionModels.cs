namespace LlamaSharpApiServer.Models.OpenAI;

public class CompletionRequest
{
    // TODO: support single item and list for 'prompt' and 'stop'
    // https://stackoverflow.com/questions/18994685/how-to-handle-both-a-single-item-and-an-array-for-the-same-property-using-json-n/18997172#18997172
    public string model { get; set; }
    public string prompt { get; set; }  // also a list
    public string? suffix { get; set; }
    public float temperature { get; set; } = 0.7f;
    public int n { get; set; } = 1;
    public int max_tokens { get; set; } = 16;
    public string? stop { get; set; } // also a list
    public bool stream { get; set; } = false;
    public float top_p { get; set; } = 1.0f;
    public int? logprobs { get; set; }
    public bool echo { get; set; } = false;
    public float presence_penalty { get; set; } = 0.0f;
    public float frequency_penalty { get; set; } = 0.0f;
    public string? user { get; set; }
}

public class CompletionResponse
{
    public string id { get; set; }
    public string _object = "text_completion";
    public long created { get; set; }
    public string model { get; set; }
    public CompletionResponseChoice[] choices { get; set; }
    public UsageInfo usage { get; set; }
}

public class CompletionResponseChoice
{
    public string text { get; set; }
    public int index { get; set; }
    public int? logprobs { get; set; }
    public string finish_reason { get; set; }
}
