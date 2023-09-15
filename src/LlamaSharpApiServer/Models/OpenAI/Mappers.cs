namespace LlamaSharpApiServer.Models.OpenAI;

using LlamaSharpApiServer.Models.Internal;

public static class Mappers
{
    /// <summary>
    /// Convert the conversation to OpenAI chat completion format
    /// </summary>
    public static List<ChatCompletionMessage> ToOpenAIApiMessages(this List<ConversationMessage> messages, string systemMessage, int offset = 0)
    {
        var ret = new List<ChatCompletionMessage>()
        {
            new ChatCompletionMessage { role = "system", content = systemMessage }
        };

        for (int i = offset; i < messages.Count; i++)
        {
            if (i % 2 == 0)
            {
                ret.Add(new ChatCompletionMessage { role = "user", content = messages[i].Message });
            }
            else
            {
                if (!string.IsNullOrEmpty(messages[i].Message))
                {
                    ret.Add(new ChatCompletionMessage { role = "assistant", content = messages[i].Message });
                }
            }
        }
        return ret;
    }
}
