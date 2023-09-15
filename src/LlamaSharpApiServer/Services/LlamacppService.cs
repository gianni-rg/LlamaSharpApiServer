namespace LlamaSharpApiServer.Services;

using LLama;
using LLama.Common;
using LlamaSharpApiServer.Extensions;
using LlamaSharpApiServer.Interfaces;
using LlamaSharpApiServer.Models.Internal;
using LlamaSharpApiServer.Models.OpenAI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

public class LlamacppService : ILLMService, IDisposable
{
    #region Private fields
    private readonly ChatSession _session;
    private readonly LLamaWeights _model;
    private readonly LLamaContext _context;

    private bool _continue = false;
    private bool _disposedValue = false;

    private string _systemPrompt;
    #endregion

    #region Constructor
    public LlamacppService()
    {
        // TODO: read params from config or an external file
        var modelPath = @"D:\Personal\GitHub\TheBloke-Llama-2-13B-chat-GGUF\llama-2-13b-chat.Q5_K_M.gguf";
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 1024,
            Seed = 1337,
            GpuLayerCount = 5
        };

        _model = LLamaWeights.LoadFromFile(parameters);
        _context = _model.CreateContext(parameters);

        // System Prompt should be defined from config or an external file
        _systemPrompt = "Transcript of a dialog, where the User interacts with an Assistant. Assistant is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.\n\n"
                      + "User: ";

        // Type of executor should be defined from config or an external file
        _session = new ChatSession(new InteractiveExecutor(_context));
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
            new ModelCard() { id = "llama-2-13b-chat.Q4_K_M.gguf", created = new DateTime(2023, 09, 04, 17, 27, 03, DateTimeKind.Utc).ToUnixTime(), owned_by = "The Bloke" },
            new ModelCard() { id = "llama-2-13b-chat.Q5_K_M.gguf", created = new DateTime(2023, 09, 04, 17, 29, 58, DateTimeKind.Utc).ToUnixTime(), owned_by = "The Bloke" }
        };

        return new ModelsResponse() { data = modelCards.ToArray() };
    }

    /// <summary>
    /// Creates a completion for the chat message
    /// </summary>
    public async Task<ChatCompletionResponse> CreateChatCompletionAsync(ChatCompletionRequest request)
    {
        //TODO: validate arguments, handle errors

        if (request.stream)
        {
            //TODO: add streaming support
            //var generator = chat_completion_stream_generator(request.model, gen_params, request.n, worker_addr);
            //return StreamingResponse(generator, media_type = "text/event-stream")
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
            new List<string> { request.stop ?? string.Empty });

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
            choices = choices,
            usage = usage
        };
    }

    public CompletionResponse CreateCompletion(CompletionRequest request)
    {
        return new CompletionResponse();
    }

    public EmbeddingsResponse CreateEmbeddings(EmbeddingsRequest request)
    {
        return new EmbeddingsResponse();
    }
    #endregion

    #region Private methods
    private GenerationParameters GetGenerationParameters(string modelName, List<ConversationMessage> messages, float temperature, float topP, int maxTokens, bool echo, List<string> stopStrings)
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

        gen_params.Stop = newStopStrings.ToList();

        return gen_params;
    }

    private Conversation GetConversationTemplate(string modelName)
    {
        if (modelName.Contains("llama-2"))
        {
            return GetLlama2Template();
        }
        else
        {
            throw new InvalidOperationException($"Unknown model: {modelName}");
        }
    }

    private CompletionResult GenerateCompletion(GenerationParameters parameters)
    {
        var outputText = new StringBuilder();
        foreach (var text in _session.Chat(parameters.Prompt, new InferenceParams() { Temperature = parameters.Temperature, AntiPrompts = new List<string> { "User:" } }))
        {
            outputText.Append(text);
        }

        var res = new CompletionResult
        {
            Text = outputText.ToString(),
            FinishReason = "stop"
        };

        var tokenizedInput = _session.Executor.Context.Tokenize(parameters.Prompt);
        var tokenizedOutput = _session.Executor.Context.Tokenize(res.Text);
        res.Usage.PromptTokens = tokenizedInput.Length;
        res.Usage.CompletionTokens = tokenizedOutput.Length;
        res.Usage.TotalTokens = tokenizedOutput.Length + tokenizedInput.Length;

        return res;
    }

    // llama2 template
    // reference: https://huggingface.co/blog/codellama#conversational-instructions
    // reference: https://github.com/facebookresearch/llama/blob/1a240688810f8036049e8da36b073f63d2ac552c/llama/generation.py#L212
    private Conversation GetLlama2Template()
    {
        return new Conversation()
        {
            Name = "llama-2",
            SystemTemplate = "[INST] <<SYS>>\n{system_message}\n<</SYS>>\n\n",
            roles = new List<string> { "[INST]", "[/INST]" },
            SepStyle = 0, //SeparatorStyle.LLAMA2,
            Sep1 = " ",
            Sep2 = " </s><s>",
        };
    }
    #endregion
}