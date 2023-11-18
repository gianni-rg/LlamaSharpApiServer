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

namespace LlamaSharpApiServer.Services;

using LLama;
using LLama.Common;
using LlamaSharpApiServer.Extensions;
using LlamaSharpApiServer.Interfaces;
using LlamaSharpApiServer.Models;
using LlamaSharpApiServer.Models.Internal;
using LlamaSharpApiServer.Models.OpenAI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class LlamacppService : ILLMService, IDisposable
{
    #region Private fields
    private readonly AppSettings _settings;
    private readonly StatelessExecutor _statelessExecutor;
    private readonly LLamaWeights _model;
    private readonly LLamaContext _context;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
    };

    //private bool _continue = false;
    private bool _disposedValue = false;
    //private readonly string _systemPrompt;

    #endregion

    #region Constructor
    public LlamacppService(AppSettings settings)
    {
        _settings = settings;

        var parameters = new ModelParams(Path.Combine(_settings.ModelsPath, _settings.Model))
        {
            ContextSize = _settings.ModelSettings.ContextSize,
            Seed = _settings.ModelSettings.Seed,
            GpuLayerCount = _settings.ModelSettings.GpuLayerCount
        };

        _model = LLamaWeights.LoadFromFile(parameters);
        _context = _model.CreateContext(parameters);

        // System Prompt should be defined from config or an external file
        //_systemPrompt = "Transcript of a dialog, where the User interacts with an Assistant. Assistant is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.\n\n"
        //              + "User: ";

        // Type of executor should be defined from config or an external file
        _statelessExecutor = new StatelessExecutor(_model, parameters);
    }
    #endregion

    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects)
                _model.Dispose();
                _context?.Dispose();
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer
            _disposedValue = true;
        }
    }

    //~LlamacppService()
    //{
    //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //    Dispose(disposing: false);
    //}

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Public methods
    public ModelsResponse ShowAvailableModels()
    {
        // TODO: currently hard-coded, but should be read from config or an external file
        var modelCards = new List<ModelCard>
        {
            new() { id = "llama-2-13b-chat.Q4_K_M.gguf", created = new DateTime(2023, 09, 04, 17, 27, 03, DateTimeKind.Utc).ToUnixTime(), owned_by = "The Bloke" },
            new() { id = "llama-2-13b-chat.Q5_K_M.gguf", created = new DateTime(2023, 09, 04, 17, 29, 58, DateTimeKind.Utc).ToUnixTime(), owned_by = "The Bloke" }
        };

        return new ModelsResponse() { data = [.. modelCards] };
    }

    /// <summary>
    /// Creates a completion for the chat message
    /// </summary>
    public async Task<ChatCompletionResponse> CreateChatCompletionAsync(ChatCompletionRequest request)
    {
        //TODO: validate arguments, handle errors

        if (request.stream)
        {
            // Handled by CreateChatCompletionStream
            throw new InvalidOperationException("CreateChatCompletionAsync called, instead of CreateChatCompletionStream");
        }

        var choices = new List<ChatCompletionResponseChoice>();
        var chat_completions_tasks = new List<Task<CompletionResult>>();

        var genParams = GetGenerationParameters(
            request.model,
            request.messages.Select(m => new ConversationMessage { Role = m.role, Message = m.content }).ToList(),
            request.temperature,
            request.top_p,
            request.max_tokens ?? 512,
            false,
            [request.stop ?? string.Empty]);


        for (int i = 0; i < request.n; i++)
        {
            chat_completions_tasks.Add(Task.Run(() => GenerateCompletion(genParams)));
        }

        CompletionResult[] chat_completion_results;
        try
        {
            chat_completion_results = await Task.WhenAll(chat_completions_tasks);
        }
        catch (Exception e)
        {
            throw new Exception("Error in CreateChatCompletion", e);
            //TODO: handle error
            //return create_error_response(ErrorCode.INTERNAL_ERROR, e);
        }

        var usage = new UsageInfo();
        for (int i = 0; i < request.n; i++)
        {
            var chat_completion_result = chat_completion_results[i];
            if (chat_completion_result.ErrorCode != 0)
            {
                throw new Exception("Error in CreateChatCompletion");
                //TODO: handle error
                //return create_error_response(content["error_code"], content["text"]);
            }
            choices.Add(new ChatCompletionResponseChoice()
            {
                index = i,
                message = new ChatCompletionMessage() { role = "assistant", content = chat_completion_result.Text },
                finish_reason = chat_completion_result.FinishReason ?? "stop"
            });

            usage.prompt_tokens = chat_completion_result.Usage.PromptTokens;
            usage.completion_tokens = chat_completion_result.Usage.CompletionTokens;
            usage.total_tokens = chat_completion_result.Usage.TotalTokens;
        }

        return new ChatCompletionResponse()
        {
            model = request.model,
            choices = [.. choices],
            usage = usage,
            created = DateTime.UtcNow.ToUnixTime()
        };
    }

    /// <summary>
    /// Creates a completion for the chat message (streaming)<br />
    /// Event stream format:<br />
    /// https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events#event_stream_format
    /// </summary>
    public async IAsyncEnumerable<string> CreateChatCompletionStream(ChatCompletionRequest request)
    {
        //TODO: validate arguments, handle errors

        //var choices = new List<ChatCompletionResponseChoice>();
        //var chat_completions_tasks = new List<Task<CompletionResult>>();
        var reqDump = JsonSerializer.Serialize(request);

        var genParams = GetGenerationParameters(
            request.model,
            request.messages.Select(m => new ConversationMessage { Role = m.role, Message = m.content }).ToList(),
            request.temperature,
            request.top_p,
            request.max_tokens ?? 512,
            false,
            [request.stop ?? string.Empty]);

        genParams.MaxNewTokens = 50; // DEBUG

        var id = $"chatcmpl-{Guid.NewGuid()}";
        //TODO: port equivalent https://github.com/skorokithakis/shortuuid in C#, like done here: https://blog.codinghorror.com/equipping-our-ascii-armor/

        var finish_stream_events = new List<ChatCompletionChunkResponse>();
        for (int i = 0; i < request.n; i++)
        {
            // First chunk with role
            var choice_data = new ChatCompletionChunkResponseChoice()
            {
                index = i,
                delta = new ChatCompletionMessage() { role = "assistant " },
                finish_reason = null
            };
            var choices = new List<ChatCompletionChunkResponseChoice>
            {
                choice_data
            };

            var chunk = new ChatCompletionChunkResponse() { id = id, choices = [.. choices], model = request.model };
            yield return $"data: {JsonSerializer.Serialize(chunk, _jsonSerializerOptions)}\n\n";

            //var previous_text = string.Empty;

            await foreach (var content in GenerateCompletionStream(genParams))
            {
                if (content.ErrorCode != 0)
                {
                    yield return $"data: {JsonSerializer.Serialize(content, _jsonSerializerOptions)}\n\n";
                    yield return "data: [DONE]\n\n";
                    yield break;
                }

                var decoded_unicode = content.Text.Replace("\ufffd", string.Empty);
                var delta_text = decoded_unicode; // decoded_unicode.Length > previous_text.Length ? decoded_unicode.Substring(previous_text.Length) : decoded_unicode;
                //previous_text = decoded_unicode.Length > previous_text.Length ? decoded_unicode : previous_text;

                if (delta_text.Length == 0)
                {
                    delta_text = string.Empty;
                }

                choice_data = new ChatCompletionChunkResponseChoice()
                {
                    index = i,
                    delta = new ChatCompletionMessage() { content = delta_text },
                    finish_reason = content.FinishReason ?? null
                };

                chunk = new ChatCompletionChunkResponse()
                {
                    id = id,
                    choices = [choice_data],
                    model = request.model
                };

                if (string.IsNullOrWhiteSpace(delta_text))
                {
                    if (content.FinishReason is not null)
                    {
                        finish_stream_events.Add(chunk);
                    }
                    continue;
                }

                yield return $"data: {JsonSerializer.Serialize(chunk, _jsonSerializerOptions)}\n\n";
            }
        }
        // There is not "content" field in the last delta message, so exclude_none to exclude field "content".
        foreach (var finish_chunk in finish_stream_events)
        {
            yield return $"data: {JsonSerializer.Serialize(finish_chunk, _jsonSerializerOptions)}\n\n";
        }
        yield return "data: [DONE]\n\n";
    }

    public async Task<CompletionResponse> CreateCompletionAsync(CompletionRequest request)
    {
        if (request.stream)
        {
            // Handled by CreateCompletionStream
            throw new InvalidOperationException("CreateCompletionAsync called, instead of CreateCompletionStream");
        }

        // TODO: validate arguments, handle errors

        //request.prompt = ProcessInput(request.model, request.prompt);

        CompletionResult[] completion_results;
        var completions_tasks = new List<Task<CompletionResult>>();
        //foreach(var text in request.prompt)
        // TODO: add support for custom serializer, currently only 1 prompt is supported
        {
            var text = request.prompt;
            var genParams = GetGenerationParameters(
                request.model,
                [new() { Role = "user", Message = text }],
                request.temperature,
                request.top_p,
                request.max_tokens,
                request.echo,
                [request.stop ?? string.Empty]);

            //genParams.MaxNewTokens = 50; // DEBUG

            for (int i = 0; i < request.n; i++)
            {
                completions_tasks.Add(Task.Run(() => GenerateCompletion(genParams)));
            }
        }//foreach

        try
        {
            completion_results = await Task.WhenAll(completions_tasks);
        }
        catch (Exception e)
        {
            throw new Exception("Error in CreateCompletion", e);
            //TODO: handle error
            //return create_error_response(ErrorCode.INTERNAL_ERROR, e);
        }

        var usage = new UsageInfo();
        var choices = new List<CompletionResponseChoice>();
        for (int i = 0; i < request.n; i++)
        {
            var completion_result = completion_results[i];
            if (completion_result.ErrorCode != 0)
            {
                throw new Exception("Error in CreateCompletion");
                //TODO: handle error
                //return create_error_response(content["error_code"], content["text"]);
            }
            choices.Add(new CompletionResponseChoice()
            {
                index = i,
                text = completion_result.Text,
                logprobs = completion_result.LogProbs ?? null,
                finish_reason = completion_result.FinishReason ?? "stop"
            });
        }

        var response = new CompletionResponse()
        {
            id = $"cmpl-{Guid.NewGuid}",
            model = request.model,
            created = DateTime.UtcNow.ToUnixTime(),
            usage = usage,
            choices = [.. choices],
        };
        return response;
    }

    /// <summary>
    /// Creates a completion for a message (streaming)<br />
    /// Event stream format:<br />
    /// https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events#event_stream_format<br />
    /// Possible .NET implementations:<br />
    /// https://itnext.io/server-side-event-streams-with-dotnet-core-and-typescript-d20c84017480<br />
    /// https://dev.to/masanori_msl/aspnet-core-try-server-sent-events-5db2
    /// </summary>
    public IAsyncEnumerable<string> CreateCompletionStream(CompletionRequest request)
    {
        throw new NotImplementedException();
    }

    public EmbeddingsResponse CreateEmbeddings(EmbeddingsRequest request)
    {
        return new EmbeddingsResponse();
    }
    #endregion

    #region Private methods
    private static GenerationParameters GetGenerationParameters(string modelName, List<ConversationMessage> messages, float temperature, float topP, int maxTokens, bool echo, List<string> stopStrings)
    {
        var convTemplate = GetConversationTemplate(modelName);

        var conversation = new Conversation()
        {
            Name = convTemplate.Name,
            SystemTemplate = convTemplate.SystemTemplate,
            SystemMessage = convTemplate.SystemMessage,
            roles = convTemplate.roles,
            Messages = new List<ConversationMessage>(convTemplate.Messages.Select(m => new ConversationMessage { Role = m.Role, Message = m.Message })),  // prevent in-place modification
            Offset = convTemplate.Offset,
            SepStyle = convTemplate.SepStyle,
            Sep1 = convTemplate.Sep1,
            Sep2 = convTemplate.Sep2,
            StopString = convTemplate.StopString,
            StopTokenIds = convTemplate.StopTokenIds
        };

        foreach (var message in messages)
        {
            var msgRole = message.Role;
            if (msgRole == "system")
            {
                conversation.SystemMessage = message.Message;
            }
            else if (msgRole == "user")
            {
                conversation.AppendMessage(conversation.roles[0], message.Message);
            }
            else if (msgRole == "assistant")
            {
                conversation.AppendMessage(conversation.roles[1], message.Message);
            }
            else
            {
                throw new InvalidOperationException($"Unknown role: {msgRole}");
            }
        }

        // Add a blank message for the assistant.
        conversation.AppendMessage(conversation.roles[1], string.Empty);

        var prompt = conversation.GetPrompt();

        var gen_params = new GenerationParameters
        {
            Model = modelName,
            Prompt = prompt,
            Temperature = temperature,
            TopP = topP,
            MaxNewTokens = maxTokens,
            Echo = echo,
            StopTokenIds = conversation.StopTokenIds
        };

        var newStopStrings = new HashSet<string>();

        foreach (var stopString in stopStrings)
        {
            if (string.IsNullOrWhiteSpace(stopString))
            {
                continue;
            }
            newStopStrings.Add(stopString);
        };

        foreach (var stopString in conversation.StopString)
        {
            if (string.IsNullOrWhiteSpace(stopString))
            {
                continue;
            }
            newStopStrings.Add(stopString);
        };

        gen_params.Stop = [.. newStopStrings];

        return gen_params;
    }

    private static Conversation GetConversationTemplate(string modelName)
    {
        if (modelName.Contains("llama-2") || modelName.Contains("gpt"))
        {
            return GetLlama2Template();
        }
        else
        {
            throw new InvalidOperationException($"Unknown model: {modelName}");
        }
    }

    private async Task<CompletionResult> GenerateCompletion(GenerationParameters parameters)
    {
        Console.WriteLine($"GenerateCompletion: {parameters.Prompt}");

        var outputText = new StringBuilder();

        await foreach (var text in _statelessExecutor.InferAsync(parameters.Prompt, new InferenceParams() { Temperature = parameters.Temperature, AntiPrompts = new List<string> { "User:" }, MaxTokens = parameters.MaxNewTokens }))
        {
            outputText.Append(text);
        }

        var res = new CompletionResult
        {
            ErrorCode = 0,
            LogProbs = null,
            Text = outputText.ToString(),
            FinishReason = "stop"
        };

        var tokenizedInput = _statelessExecutor.Context.Tokenize(parameters.Prompt);
        var tokenizedOutput = _statelessExecutor.Context.Tokenize(res.Text);
        res.Usage.PromptTokens = tokenizedInput.Length;
        res.Usage.CompletionTokens = tokenizedOutput.Length;
        res.Usage.TotalTokens = tokenizedOutput.Length + tokenizedInput.Length;

        return res;
    }

    private async IAsyncEnumerable<CompletionResult> GenerateCompletionStream(GenerationParameters parameters)
    {
        Console.WriteLine($"GenerateCompletionStream: {parameters.Prompt}");

        //var outputs = _session.ChatAsync(parameters.Prompt, new InferenceParams() { Temperature = parameters.Temperature, AntiPrompts = new List<string> { "User:" } });
        var outputs = _statelessExecutor.InferAsync(parameters.Prompt, new InferenceParams() { Temperature = parameters.Temperature, AntiPrompts = new List<string> { "User:" }, MaxTokens = parameters.MaxNewTokens });

        await foreach (var output in outputs)
        {
            var res = new CompletionResult
            {
                ErrorCode = 0,
                LogProbs = null,
                Text = output,
                FinishReason = null
            };

            yield return res;
        }

        var finalRes = new CompletionResult
        {
            ErrorCode = 0,
            LogProbs = null,
            Text = string.Empty,
            FinishReason = "stop"
        };

        yield return finalRes;
    }

    // llama2 template
    // reference: https://huggingface.co/blog/codellama#conversational-instructions
    // reference: https://github.com/facebookresearch/llama/blob/1a240688810f8036049e8da36b073f63d2ac552c/llama/generation.py#L212
    private static Conversation GetLlama2Template()
    {
        return new Conversation()
        {
            Name = "llama-2",
            SystemTemplate = "[INST] <<SYS>>\n{system_message}\n<</SYS>>\n\n",
            roles = ["[INST]", "[/INST]"],
            SepStyle = 0, //SeparatorStyle.LLAMA2,
            Sep1 = " ",
            Sep2 = " </s><s>",
        };
    }
    #endregion
}