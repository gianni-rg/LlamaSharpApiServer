namespace LlamaSharpApiServer.Models;

public class EmbeddingsRequest
{
    string input { get; set; }
    string model { get; set; }
}

public class EmbeddingsResponse
{
    public string _object { get; set; }
    public EmbeddingsResponseDatum[] data { get; set; }
    public string model { get; set; }
    public UsageInfo usage { get; set; }
}

public class EmbeddingsResponseDatum
{
    public string _object { get; set; }
    public float[] embedding { get; set; }
    public int index { get; set; }
}

