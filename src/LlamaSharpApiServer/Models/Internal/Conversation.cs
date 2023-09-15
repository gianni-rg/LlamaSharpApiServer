namespace LlamaSharpApiServer.Models.Internal;

/// <summary>
/// Represents a message in a conversation
/// </summary>
public record ConversationMessage
{
    public string Role { get; set; }
    public string Message { get; set; }
}


/// <summary>
/// Manages prompt templates and keeps all conversation history
/// </summary>
public class Conversation
{
    /// <summary>
    /// The name of this template
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The template of the system prompt
    /// </summary>
    public string SystemTemplate { get; set; } = "{system_message}";
    /// <summary>
    /// The system message
    /// </summary>
    public string SystemMessage { get; set; } = string.Empty;
    /// <summary>
    /// The names of two roles
    /// </summary>
    public List<string> roles = new List<string> { "USER", "ASSISTANT" };
    /// <summary>
    /// All messages in the conversation
    /// </summary>
    public List<ConversationMessage> Messages { get; set; } = new List<ConversationMessage>();
    /// <summary>
    /// The number of few shot examples
    /// </summary>
    public int Offset { get; set; } = 0;
    /// <summary>
    /// The separator style and configurations
    /// </summary>
    public int SepStyle { get; set; } = 0; //SeparatorStyle.LLAMA2;
    public string Sep1 { get; set; } = string.Empty;
    public string Sep2 { get; set; } = string.Empty;
    /// <summary>
    /// Stop criteria (the default one is EOS token)
    /// </summary>
    public List<string> StopString { get; set; } = new List<string>();
    /// <summary>
    /// Stops generation if meeting any token in this list
    /// </summary>
    public List<int> StopTokenIds { get; set; } = new List<int>();

    /// <summary>
    /// Get the prompt for generation.
    /// </summary>
    /// <returns></returns>
    public string GetPrompt()
    {
        string ret = string.Empty;

        var system_prompt = SystemMessage;

        if (SepStyle == 0 /* SeparatorStyle.LLAMA2 */)
        {
            var seps = new List<string> { Sep1, Sep2 };
            if (!string.IsNullOrWhiteSpace(SystemMessage))
            {
                ret = system_prompt;
            }
            else
            {
                ret = "[INST] ";
            }
            for (int i = 0; i < Messages.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(Messages[i].Message))
                {
                    if (i == 0)
                    {
                        ret += Messages[i].Message + " ";
                    }
                    else
                    {
                        ret += Messages[i].Role + " " + Messages[i].Message + seps[i % 2];
                    }
                }
                else
                {
                    ret += Messages[i].Role;
                }
            }
        }
        return ret;
    }

    /// <summary>
    /// Append a new message
    /// </summary>
    public void AppendMessage(string role, string message)
    {
        Messages.Add(new ConversationMessage { Role = role, Message = message });
    }

    /// <summary>
    /// Append a new message
    /// </summary>
    public void AppendMessage(ConversationMessage message)
    {
        Messages.Add(message);
    }
}
