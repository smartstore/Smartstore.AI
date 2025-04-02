#nullable enable

namespace Smartstore.AI.GeminiClient
{
    /// <summary>
    /// <see cref="https://ai.google.dev/api/files#v1beta.Status"/>
    /// </summary>
    public class GeminiStatus
    {
        public int Code { get; set; }
        
        public string? Message { get; set; }
    }
}
