#nullable enable

namespace Smartstore.AI.GeminiClient
{
    /// <summary>
    /// <see cref="https://ai.google.dev/api/generate-content#generatecontentresponse" />
    /// </summary>
    public class GeminiGenerateContentResponse
    {
        public List<GeminiCandidate> Candidates { get; set; } = [];

        public GeminiPromptFeedback? PromptFeedback { get; set; }

        public string? ModelVersion { get; set; }

        public override string ToString()
            => string.Join(Environment.NewLine, Candidates.Select(x => x.ToString()));
    }

    public class GeminiCandidate
    {
        /// <summary>
        /// The generated content returned from the model.
        /// </summary>
        public GeminiContent? Content { get; set; }

        /// <summary>
        /// The reason why the model stopped generating tokens.
        /// </summary>
        public string? FinishReason { get; set; }

        /// <summary>
        /// The index of the candidate in the list of response candidates.
        /// </summary>
        public int Index { get; set; }

        public override string ToString()
            => Content?.ToString() + (FinishReason != null ? $" finish:{FinishReason}" : string.Empty);
    }
}
