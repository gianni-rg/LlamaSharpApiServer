namespace LlamaSharpApiServer.Models;

public class ChatCompletionRequest
{
    public string model { get; set; }
    public ChatCompletionMessage[] messages { get; set; }
    public ChatCompletionFunction[]? functions { get; set; }
    public string? function_call { get; set; }
    public float temperature { get; set; } = 1.0f;
    public float top_p { get; set; } = 1.0f;
    public int n { get; set; } = 1;
    public bool stream { get; set; } = false;
    public string? stop { get; set; } = null;
    public int? max_tokens { get; set; } = null;
    public float presence_penalty { get; set; } = 0.0f;
    public float frequency_penalty { get; set; } = 0.0f;
    public string? user { get; set; } = null;
}

public class ChatCompletionResponse
{
    public string id { get; set; }
    public string _object = "chat.completion";
    public int created { get; set; }
    public string model { get; set; }
    public ChatCompletionResponseChoice[] choices { get; set; }
    public UsageInfo usage { get; set; }
}

public class ChatCompletionResponseChoice
{
    public int index { get; set; }
    public ChatCompletionMessage message { get; set; }
    public string finish_reason { get; set; }
}

public class ChatCompletionFunction
{
    public string name { get; set; }
    public string? description { get; set; }
    public ChatCompletionFunctionParameters parameters { get; set; }
}


public class ChatCompletionFunctionParameters
{
    public string type { get; set; }
    public ChatCompletionFunctionParametersProperties properties { get; set; }
    public string[] required { get; set; }
}

public class ChatCompletionFunctionParametersProperties
{
    public ChatCompletionFunctionParametersPropertiesLocation location { get; set; }
    public ChatCompletionFunctionParametersPropertiesUnit unit { get; set; }
}

public class ChatCompletionFunctionParametersPropertiesLocation
{
    public string type { get; set; }
    public string description { get; set; }
}

public class ChatCompletionFunctionParametersPropertiesUnit
{
    public string type { get; set; }
    public string[] _enum { get; set; }
}



public class ChatCompletionMessage
{
    public string role { get; set; }
    public string? content { get; set; }
    public string? name { get; set; }
    public ChatCompletionFunctionCall? function_call { get; set; }
}

public class ChatCompletionFunctionCall
{
    public string name { get; set; }
    public string arguments { get; set; }
}

public class ChatCompletionChunkResponse
{
    public string id { get; set; }
    public string _object => "chat.completion.chunk";
    public int created { get; set; }
    public string model { get; set; }
    public ChatCompletionChunkResponseChoice[] choices { get; set; }
}

public class ChatCompletionChunkResponseChoice
{
    public int index { get; set; }
    public ChatCompletionDeltaMessage delta { get; set; }
    public string finish_reason { get; set; }
}

public class ChatCompletionDeltaMessage
{
    public string content { get; set; }
}
