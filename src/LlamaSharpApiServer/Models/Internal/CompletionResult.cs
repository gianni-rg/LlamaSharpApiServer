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

/// <summary>
/// Represents a completion result from the model
/// </summary>
public class CompletionResult
{
    public string Text { get; set; } = string.Empty;
    public int? LogProbs { get; set; } = null;
    public UsageInformation Usage { get; set; } = new UsageInformation();
    public string? FinishReason { get; set; } = null;
    public int ErrorCode { get; set; } = 0;
}
