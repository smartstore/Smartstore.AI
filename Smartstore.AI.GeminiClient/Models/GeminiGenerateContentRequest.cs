#nullable enable

namespace Smartstore.AI.GeminiClient
{
    /// <summary>
    /// <see cref="https://ai.google.dev/api/generate-content#request-body" />
    /// </summary>
    public class GeminiGenerateContentRequest
    {
        public required List<GeminiContent> Contents { get; set; }

        public GeminiContent? SystemInstruction { get; set; }

        public GeminiGenerationConfig? GenerationConfig { get; set; }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Contents.Where(x => x.Role != "model").Select(x => x.ToString()))
                + Environment.NewLine 
                + $"system: {string.Join(Environment.NewLine, SystemInstruction?.Parts.Select(x => x.Text) ?? [])}"
                + Environment.NewLine
                + string.Join(Environment.NewLine, Contents.Where(x => x.Role == "model").Select(x => x.ToString()));
        }
    }
}
