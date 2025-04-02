#nullable enable

namespace Smartstore.AI.GeminiClient
{
    public class GeminiErrorResponse
    {
        public GeminiError? Error { get; set; }

        public override string ToString()
            => Error?.ToString() ?? string.Empty;
    }

    public class GeminiError
    {
        public int Code { get; set; }

        public string? Message { get; set; }

        public string? Status { get; set; }

        public override string ToString()
            => $"{Message} ({Code} {Status})";
    }
}
