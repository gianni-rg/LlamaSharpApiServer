namespace LlamaSharpApiServer.Models.OpenAI;

public class UsageInfo
{
    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
}

public class ModelsRequest
{
}

public class ModelsResponse
{
    public string _object => "list";
    public ModelCard[] data { get; set; }
}

public class ModelCard
{
    public string id { get; set; }
    public string _object => "model";
    public long created { get; set; }
    public string owned_by { get; set; }
}
