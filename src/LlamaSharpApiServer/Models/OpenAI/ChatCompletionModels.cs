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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable IDE1006 // Naming Styles

namespace LlamaSharpApiServer.Models.OpenAI;

public class ChatCompletionRequest
{
    public string model { get; set; } = string.Empty;
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
    public string id { get; set; } = string.Empty;
    public string _object = "chat.completion";
    public long created { get; set; }
    public string model { get; set; } = string.Empty;
    public ChatCompletionResponseChoice[] choices { get; set; }
    public UsageInfo usage { get; set; }
}

public class ChatCompletionResponseChoice
{
    public int index { get; set; }
    public ChatCompletionMessage message { get; set; }
    public string finish_reason { get; set; } = string.Empty;
}

public class ChatCompletionFunction
{
    public string name { get; set; } = string.Empty;
    public string? description { get; set; }
    public ChatCompletionFunctionParameters parameters { get; set; }
}


public class ChatCompletionFunctionParameters
{
    public string type { get; set; } = string.Empty;
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
    public string type { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
}

public class ChatCompletionFunctionParametersPropertiesUnit
{
    public string type { get; set; } = string.Empty;
    public string[] _enum { get; set; }
}



public class ChatCompletionMessage
{
    public string role { get; set; } = string.Empty;
    public string content { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public ChatCompletionFunctionCall? function_call { get; set; }
}

public class ChatCompletionFunctionCall
{
    public string name { get; set; } = string.Empty;
    public string arguments { get; set; } = string.Empty;
}


public class ChatCompletionChunkResponse
{
    public string id { get; set; } = string.Empty;
    public string _object = "chat.completion.chunk";
    public int created { get; set; }
    public string model { get; set; } = string.Empty;
    public ChatCompletionChunkResponseChoice[] choices { get; set; }
}

public class ChatCompletionChunkResponseChoice
{
    public int index { get; set; }
    public ChatCompletionMessage delta { get; set; }
    public string? finish_reason { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore IDE1006 // Naming Styles