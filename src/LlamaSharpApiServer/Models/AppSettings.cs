namespace LlamaSharpApiServer.Models;

public class AppSettings
{
    public int ServerPort { get; set; }
    public string ModelsPath { get; set; }
    public string Model { get; set; }
    public ModelSettings ModelSettings { get; set; }
}
