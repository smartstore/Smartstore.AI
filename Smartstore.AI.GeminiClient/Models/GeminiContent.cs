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

    /// <summary>
    /// A datatype containing media that is part of a multi-part Content message.
    /// Either <see cref="Text" />, <see cref="InlineData" /> or <see cref="FileData" /> must be provided.
    /// </summary>
    public class GeminiPart
    {
        /// <summary>
        /// Inline text.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Inline media data.
        /// </summary>
        public GeminiInlineData? InlineData { get; set; }

        /// <summary>
        /// URI based data.
        /// </summary>
        public GeminiFileData? FileData { get; set; }
    }

    public class GeminiInlineData
    {
        /// <summary>
        /// The IANA standard MIME type of the source data. 
        /// </summary>
        /// <example>image/jpeg</example>
        public required string MimeType { get; set; }

        /// <summary>
        /// A base64-encoded string representing the raw media data.
        /// </summary>
        public required string Data { get; set; }
    }
}
