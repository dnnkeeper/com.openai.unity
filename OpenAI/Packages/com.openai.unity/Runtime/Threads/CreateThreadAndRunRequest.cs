using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;

namespace OpenAI.Threads
{
    [Preserve]
    public sealed class CreateThreadAndRunRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="assistantId">
        /// The ID of the assistant to use to execute this run.
        /// </param>
        /// <param name="request"><see cref="CreateThreadAndRunRequest"/>.</param>
        [Preserve]
        public CreateThreadAndRunRequest(string assistantId, CreateThreadAndRunRequest request)
            : this(
                assistantId,
                request?.Model,
                request?.Instructions,
                request?.Tools,
                request?.ToolResources,
                request?.Metadata,
                request?.Temperature,
                request?.TopP,
                request?.Stream ?? false,
                request?.MaxPromptTokens,
                request?.MaxCompletionTokens,
                request?.TruncationStrategy,
                request?.ToolChoice as string ?? (string)request?.ToolChoice?.funcion?.name,
                request?.ResponseFormat ?? OpenAI.ResponseFormat.Text)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="assistantId">
        /// The ID of the assistant to use to execute this run.
        /// </param>
        /// <param name="model">
        /// The ID of the Model to be used to execute this run.
        /// If a value is provided here, it will override the model associated with the assistant.
        /// If not, the model associated with the assistant will be used.
        /// </param>
        /// <param name="instructions">
        /// Override the default system message of the assistant.
        /// This is useful for modifying the behavior on a per-run basis.
        /// </param>
        /// <param name="tools">
        /// Override the tools the assistant can use for this run.
        /// This is useful for modifying the behavior on a per-run basis.
        /// </param>
        /// <param name="toolResources">
        /// A set of resources that are used by the assistant's tools.
        /// The resources are specific to the type of tool.
        /// For example, the 'code_interpreter' tool requires a list of file IDs,
        /// while the 'file_search' tool requires a list of vector store IDs.
        /// </param>
        /// <param name="metadata">
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </param>
        /// <param name="temperature">
        /// What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output
        /// more random, while lower values like 0.2 will make it more focused and deterministic.
        /// When null the default temperature (1) will be used.
        /// </param>
        /// <param name="topP">
        /// An alternative to sampling with temperature, called nucleus sampling,
        /// where the model considers the results of the tokens with top_p probability mass.
        /// So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// We generally recommend altering this or temperature but not both.
        /// </param>
        /// <param name="stream">
        /// If true, returns a stream of events that happen during the Run as server-sent events,
        /// terminating when the Run enters a terminal state with a 'data: [DONE]' message.
        /// </param>
        /// <param name="maxPromptTokens">
        /// The maximum number of prompt tokens that may be used over the course of the run.
        /// The run will make a best effort to use only the number of prompt tokens specified,
        /// across multiple turns of the run. If the run exceeds the number of prompt tokens specified,
        /// the run will end with status 'incomplete'. See 'incomplete_details' for more info.
        /// </param>
        /// <param name="maxCompletionTokens">
        /// The maximum number of completion tokens that may be used over the course of the run.
        /// The run will make a best effort to use only the number of completion tokens specified,
        /// across multiple turns of the run. If the run exceeds the number of completion tokens specified,
        /// the run will end with status 'incomplete'. See 'incomplete_details' for more info.
        /// </param>
        /// <param name="truncationStrategy">
        /// Controls for how a thread will be truncated prior to the run.
        /// Use this to control the initial context window of the run.
        /// </param>
        /// <param name="toolChoice">
        /// Controls which (if any) tool is called by the model.
        /// none means the model will not call any tools and instead generates a message.
        /// auto is the default value and means the model can pick between generating a message or calling one or more tools.
        /// required means the model must call one or more tools before responding to the user.
        /// Specifying a particular tool like {"type": "file_search"} or {"type": "function", "function": {"name": "my_function"}}
        /// forces the model to call that tool.
        /// </param>
        /// <param name="responseFormat">
        /// An object specifying the format that the model must output.
        /// Setting to <see cref="ResponseFormat.Json"/> enables JSON mode,
        /// which guarantees the message the model generates is valid JSON.<br/>
        /// Important: When using JSON mode you must still instruct the model to produce JSON yourself via some conversation message,
        /// for example via your system message. If you don't do this, the model may generate an unending stream of
        /// whitespace until the generation reaches the token limit, which may take a lot of time and give the appearance
        /// of a "stuck" request. Also note that the message content may be partial (i.e. cut off) if finish_reason="length",
        /// which indicates the generation exceeded max_tokens or the conversation exceeded the max context length.
        /// </param>
        /// <param name="createThreadRequest">
        /// Optional, <see cref="CreateThreadRequest"/>.
        /// </param>
        [Preserve]
        public CreateThreadAndRunRequest(
            string assistantId,
            string model = null,
            string instructions = null,
            IReadOnlyList<Tool> tools = null,
            ToolResources toolResources = null,
            IReadOnlyDictionary<string, string> metadata = null,
            double? temperature = null,
            double? topP = null,
            bool stream = false,
            int? maxPromptTokens = null,
            int? maxCompletionTokens = null,
            TruncationStrategy truncationStrategy = null,
            string toolChoice = null,
            ResponseFormat responseFormat = OpenAI.ResponseFormat.Text,
            CreateThreadRequest createThreadRequest = null)
        {
            AssistantId = assistantId;
            Model = model;
            Instructions = instructions;

            var toolList = tools?.ToList();

            if (toolList != null && toolList.Any())
            {
                if (string.IsNullOrWhiteSpace(toolChoice))
                {
                    ToolChoice = "auto";
                }
                else
                {
                    if (!toolChoice.Equals("none") &&
                        !toolChoice.Equals("required") &&
                        !toolChoice.Equals("auto"))
                    {
                        var tool = toolList.FirstOrDefault(t => t.Function.Name.Contains(toolChoice)) ??
                                   throw new ArgumentException($"The specified tool choice '{toolChoice}' was not found in the list of tools");
                        ToolChoice = new { type = "function", function = new { name = tool.Function.Name } };
                    }
                    else
                    {
                        ToolChoice = toolChoice;
                    }
                }
            }

            Tools = toolList?.ToList();
            ToolResources = toolResources;
            Metadata = metadata;
            Temperature = temperature;
            TopP = topP;
            Stream = stream;
            MaxPromptTokens = maxPromptTokens;
            MaxCompletionTokens = maxCompletionTokens;
            TruncationStrategy = truncationStrategy;
            ResponseFormat = responseFormat;
            ThreadRequest = createThreadRequest;
        }

        /// <summary>
        /// The ID of the assistant to use to execute this run.
        /// </summary>
        [Preserve]
        [JsonProperty("assistant_id")]
        public string AssistantId { get; }

        /// <summary>
        /// The ID of the Model to be used to execute this run.
        /// If a value is provided here, it will override the model associated with the assistant.
        /// If not, the model associated with the assistant will be used.
        /// </summary>
        [Preserve]
        [JsonProperty("model")]
        public string Model { get; }

        /// <summary>
        /// Override the default system message of the assistant.
        /// This is useful for modifying the behavior on a per-run basis.
        /// </summary>
        [Preserve]
        [JsonProperty("instructions")]
        public string Instructions { get; }

        /// <summary>
        /// The list of tools that the assistant used for this run.
        /// </summary>
        [Preserve]
        [JsonProperty("tools")]
        public IReadOnlyList<Tool> Tools { get; }

        /// <summary>
        /// A set of resources that are used by the assistant's tools.
        /// The resources are specific to the type of tool.
        /// For example, the 'code_interpreter' tool requires a list of file IDs,
        /// while the 'file_search' tool requires a list of vector store IDs.
        /// </summary>
        [Preserve]
        [JsonProperty("tool_resources")]
        public ToolResources ToolResources { get; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [Preserve]
        [JsonProperty("metadata")]
        public IReadOnlyDictionary<string, string> Metadata { get; }

        /// <summary>
        /// What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output
        /// more random, while lower values like 0.2 will make it more focused and deterministic.
        /// When null the default temperature (1) will be used.
        /// </summary>
        [Preserve]
        [JsonProperty("temperature")]
        public double? Temperature { get; }

        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling,
        /// where the model considers the results of the tokens with top_p probability mass.
        /// So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// We generally recommend altering this or temperature but not both.
        /// </summary>
        [Preserve]
        [JsonProperty("top_p")]
        public double? TopP { get; }

        /// <summary>
        /// If true, returns a stream of events that happen during the Run as server-sent events,
        /// terminating when the Run enters a terminal state with a 'data: [DONE]' message.
        /// </summary>
        [Preserve]
        [JsonProperty("stream")]
        public bool Stream { get; internal set; }

        /// <summary>
        /// The maximum number of prompt tokens that may be used over the course of the run.
        /// The run will make a best effort to use only the number of prompt tokens specified,
        /// across multiple turns of the run. If the run exceeds the number of prompt tokens specified,
        /// the run will end with status 'incomplete'. See 'incomplete_details' for more info.
        /// </summary>
        [Preserve]
        [JsonProperty("max_prompt_tokens")]
        public int? MaxPromptTokens { get; }

        /// <summary>
        /// The maximum number of completion tokens that may be used over the course of the run.
        /// The run will make a best effort to use only the number of completion tokens specified,
        /// across multiple turns of the run. If the run exceeds the number of completion tokens specified,
        /// the run will end with status 'incomplete'. See 'incomplete_details' for more info.
        /// </summary>
        [Preserve]
        [JsonProperty("max_completion_tokens")]
        public int? MaxCompletionTokens { get; }

        /// <summary>
        /// Controls for how a thread will be truncated prior to the run.
        /// Use this to control the initial context window of the run.
        /// </summary>
        [Preserve]
        [JsonProperty("truncation_strategy")]
        public TruncationStrategy TruncationStrategy { get; }

        /// <summary>
        /// Controls which (if any) tool is called by the model.
        /// none means the model will not call any tools and instead generates a message.
        /// auto is the default value and means the model can pick between generating a message or calling one or more tools.
        /// required means the model must call one or more tools before responding to the user.
        /// Specifying a particular tool like {"type": "file_search"} or {"type": "function", "function": {"name": "my_function"}}
        /// forces the model to call that tool.
        /// </summary>
        [Preserve]
        [JsonProperty("tool_choice")]
        public dynamic ToolChoice { get; }

        /// <summary>
        /// An object specifying the format that the model must output.
        /// Setting to <see cref="ResponseFormat.Json"/> enables JSON mode,
        /// which guarantees the message the model generates is valid JSON.
        /// </summary>
        /// <remarks>
        /// Important: When using JSON mode you must still instruct the model to produce JSON yourself via some conversation message,
        /// for example via your system message. If you don't do this, the model may generate an unending stream of
        /// whitespace until the generation reaches the token limit, which may take a lot of time and give the appearance
        /// of a "stuck" request. Also note that the message content may be partial (i.e. cut off) if finish_reason="length",
        /// which indicates the generation exceeded max_tokens or the conversation exceeded the max context length.
        /// </remarks>
        [Preserve]
        [JsonProperty("response_format")]
        public ResponseFormatObject ResponseFormat { get; private set; }

        /// <summary>
        /// The optional <see cref="CreateThreadRequest"/> options to use.
        /// </summary>
        [Preserve]
        [JsonProperty("thread")]
        public CreateThreadRequest ThreadRequest { get; }
    }
}
