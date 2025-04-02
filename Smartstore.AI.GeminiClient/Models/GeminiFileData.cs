#nullable enable

namespace Smartstore.AI.GeminiClient
{
    /// <summary>
    /// <see cref="https://ai.google.dev/api/caching#FileData" />
    /// </summary>
    public class GeminiFileData
    {
        /// <summary>
        /// A file URI hosted by Gemini.
        /// </summary>
        /// <remarks>
        /// The image must be hosted by Gemini or the following error will occur:
        /// "Invalid or unsupported file uri: ... (400 INVALID_ARGUMENT)"
        /// </remarks>
        public required string FileUri { get; set; }

        public string? MimeType { get; set; }
    }
}
