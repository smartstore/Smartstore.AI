namespace Smartstore.AI.GeminiClient
{
    /// <summary>
    /// <see cref="https://ai.google.dev/api/generate-content#v1beta.GenerationConfig" />
    /// </summary>
    public class GeminiGenerationConfig
    {
        public List<string>? StopSequences { get; set; }

        /// <summary>
        /// Allowed mime types are 'text/plain', 'application/json', 'application/xml', 'application/yaml', 'text/x.enum'.
        /// </summary>
        /// <remarks>
        /// Unfortunately, does not support text/html at the moment which is required to prevent wrapping of Markdown code blocks when generating HTML.
        /// </remarks>
        public string? ResponseMimeType { get; set; }

        /// <summary>
        /// Represents the set of modalities that the model can return, and should be expected in the response. 
        /// This is an exact match to the modalities of the response.
        /// An empty list is equivalent to requesting only text.
        /// <see cref="https://ai.google.dev/api/generate-content#Modality"/>
        /// </summary>
        public List<string>? ResponseModalities { get; set; }

        /// <summary>
        /// Default: 1.
        /// </summary>
        public int? CandidateCount { get; set; }

        public int? MaxOutputTokens { get; set; }

        public float? Temperature { get; set; }

        public float? TopP { get; set; }
        public int? TopK { get; set; }
        public int? Seed { get; set; }

        public bool? EnableEnhancedCivicAnswers { get; set; }

        /// <summary>
        /// <see cref="https://ai.google.dev/api/generate-content#MediaResolution"/>
        /// </summary>
        public string? MediaResolution { get; set; }

        /// <summary>
        /// Config for image generation if supported by model.
        /// <see cref="https://ai.google.dev/api/generate-content#ImageConfig"/>
        /// </summary>
        public GeminiImageConfig? ImageConfig { get; set; }
    }

    public class GeminiImageConfig
    {
        public string? AspectRatio { get; set; }
    }
}
