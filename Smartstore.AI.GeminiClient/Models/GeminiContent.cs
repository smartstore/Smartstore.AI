#nullable enable

namespace Smartstore.AI.GeminiClient
{
    /// <summary>
    /// <see cref="https://ai.google.dev/api/caching#Content" />
    /// </summary>
    public class GeminiContent
    {
        /// <summary>
        /// Must be either 'user' or 'model'.
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Ordered parts that constitute a single message. Parts may have different MIME types.
        /// </summary>
        public required List<GeminiPart> Parts { get; set; }

        public override string ToString()
            => $"{Role ?? "-"}: {string.Join(", ", Parts.Select(x => x.Text))}";
    }

    public class GeminiPart
    {
        public string? Text { get; set; }

        public GeminiInlineData? InlineData { get; set; }

        public GeminiFileData? FileData { get; set; }
    }

    public class GeminiInlineData
    {
        //[JsonPropertyName("mime_type")]
        public required string MimeType { get; set; }

        public required string Data { get; set; }
    }
}
