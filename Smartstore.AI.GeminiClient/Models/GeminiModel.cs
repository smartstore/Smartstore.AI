namespace Smartstore.AI.GeminiClient
{
    /// <summary>
    /// <see cref="https://ai.google.dev/api/models#response-body_1"/>
    /// </summary>
    public class GeminiModels
    {
        public required List<GeminiModel> Models { get; set; }

        /// <summary>
        /// A token, which can be sent as pageToken to retrieve the next page.
        /// If this field is omitted, there are no more pages.
        /// </summary>
        public string? NextPageToken { get; set; }

        public override string ToString()
            => string.Join(Environment.NewLine, Models?.Select(x => x.ToString()) ?? []);
    }

    /// <summary>
    /// <see cref="https://ai.google.dev/api/models#Model"/>
    /// </summary>
    public sealed class GeminiModel
    {
        /// <summary>
        /// The name of the model.
        /// </summary>
        /// <example>models/gemini-1.5-flash-001</example>
        public required string Name { get; set; }

        /// <summary>
        /// The name of the base model, pass this to the generation request.
        /// </summary>
        /// <example>gemini-1.5-flash</example>
        public string? BaseModelId { get; set; }

        /// <summary>
        /// The major version number of the model.
        /// </summary>
        /// <example>1.0</example>
        public string? Version { get; set; }

        /// <summary>
        /// The human-readable name of the model.
        /// </summary>
        /// <example>Gemini 1.5 Flash</example>
        public string? DisplayName { get; set; }

        public string? Description { get; set; }

        /// <summary>
        /// Maximum number of input tokens allowed for this model.
        /// </summary>
        public int? InputTokenLimit { get; set; }

        /// <summary>
        /// Maximum number of output tokens available for this model.
        /// </summary>
        public int? OutputTokenLimit { get; set; }

        /// <summary>
        /// The model's supported generation methods.
        /// </summary>
        /// <example>["generateMessage", "generateContent"]</example>
        public IList<string>? SupportedGenerationMethods { get; set; }

        /// <summary>
        /// Indicates whether the model supports thinking.
        /// </summary>
        public bool? Thinking { get; set; }

        /// <summary>
        /// Controls the randomness of the output. Values can range over[0.0, maxTemperature], inclusive. 
        /// A higher value will produce responses that are more varied, while a value closer to 0.0 will typically 
        /// result in less surprising responses from the model.
        /// </summary>
        public double? Temperature { get; set; }

        /// <summary>
        /// The maximum temperature this model can use.
        /// </summary>
        public double? MaxTemperature { get; set; }

        /// <summary>
        /// Nucleus sampling considers the smallest set of tokens whose probability sum is at least topP.
        /// </summary>
        public double? TopP { get; set; }

        /// <summary>
        /// Top-k sampling considers the set of topK most probable tokens.
        /// If empty, indicates the model doesn't use top-k sampling, and topK isn't allowed as a generation parameter.
        /// </summary>
        public int? TopK { get; set; }

        public override string ToString()
            => $"name:{Name} version:{Version} baseModelId:{BaseModelId} displayName:{DisplayName}";
    }
}
