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

public class EmbeddingsRequest
{
    public string input { get; set; } = string.Empty;
    public string model { get; set; } = string.Empty;
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

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore IDE1006 // Naming Styles