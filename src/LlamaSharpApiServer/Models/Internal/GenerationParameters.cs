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

namespace LlamaSharpApiServer.Models.Internal;

public class GenerationParameters
{
    public string Model { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public float Temperature { get; set; }
    public float TopP { get; set; }
    public int MaxNewTokens { get; set; }
    public bool Echo { get; set; }
    public List<string> Stop { get; set; } = [];
    public List<int> StopTokenIds { get; set; } = [];

}