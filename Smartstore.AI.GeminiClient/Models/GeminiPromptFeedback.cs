#nullable enable

namespace Smartstore.AI.GeminiClient
{
    /// <summary>
    /// <see cref="https://ai.google.dev/api/generate-content#PromptFeedback" />
    /// </summary>
    public class GeminiPromptFeedback
    {
        public string? BlockReason { get; set; }

        public List<GeminiSafetyRating> SafetyRatings { get; set; } = [];
    }

    public class GeminiSafetyRating
    {
        public required string Category { get; set; }

        public required string Probability { get; set; }

        public bool Blocked { get; set; }
    }
}
