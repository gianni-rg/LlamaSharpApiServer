// Copyright (C) 2023 Gianni Rosa Gallina. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License").
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Ported/inspired from LM-SYS FastChat project
// (https://github.com/lm-sys/FastChat)
// Copyright (C) 2023 LM-SYS team

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
            new() { role = "system", content = systemMessage }
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
