#nullable enable

namespace Smartstore.AI.GeminiClient
{
    public class GeminiConfig
    {
        public GeminiConfig(string apiKey, string modelName, string? baseUrl = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(apiKey);
            ArgumentException.ThrowIfNullOrEmpty(modelName);

            ApiKey = apiKey;
            ModelName = modelName;
            BaseUrl = string.IsNullOrEmpty(baseUrl) ? null : baseUrl;
        }

        public string ApiKey { get; }
        public string? BaseUrl { get; }

        /// <summary>
        /// Gets the name of the Gemini model.
        /// </summary>
        /// <example>gemini-2.0-flash</example>
        public string ModelName { get; }
    }
}
