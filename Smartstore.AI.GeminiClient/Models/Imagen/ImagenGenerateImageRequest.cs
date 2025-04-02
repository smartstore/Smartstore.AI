#nullable enable

namespace Smartstore.AI.GeminiClient
{
    /// <summary>
    /// <see cref="https://ai.google.dev/gemini-api/docs/image-generation#imagen" />
    /// <seealso cref="https://cloud.google.com/vertex-ai/generative-ai/docs/model-reference/imagen-api"/>
    /// </summary>
    public class ImagenGenerateImageRequest
    {
        public required List<ImagenGenerateImageInstance> Instances { get; set; }

        public required ImagenGenerateImageParameters Parameters { get; set; }
    }

    public class ImagenGenerateImageInstance
    {
        public required string Prompt { get; set; }
    }

    public class ImagenGenerateImageParameters
    {
        public int SampleCount { get; set; } = 1;
        
        public int? Seed { get; set; }
        
        public bool? EnhancePrompt { get; set; }

        /// <summary>
        /// A description of what to discourage in the generated images.
        /// </summary>
        public string? NegativePrompt { get; set; }

        /// <summary>
        /// 1:1:  1024 x 1024 pixels (square)
        /// 3:4:  896 x 1280
        /// 4:3:  1280 x 896
        /// 9:16: 768 x 1408
        /// 16:9: 1408 x 768
        /// Default: 1:1.
        /// </summary>
        public string? AspectRatio { get; set; }

        public ImagenGenerateImageOutputOptions? OutputOptions { get; set; }

        /// <summary>
        /// Contrary to the documentation, this property is not currently supported by the API.
        /// Possible value: 'photograph', 'digital_art', 'landscape', 'sketch', 'watercolor', 'cyberpunk', 'pop_art'.
        /// </summary>
        public string? SampleImageStyle { get; set; }

        public bool? AddWatermark { get; set; }

        public string? StorageUri { get; set; }

        #region Safety

        /// <summary>
        /// 'block_low_and_above','block_medium_and_above', 'block_only_high', 'block_none'.
        /// Default: 'block_medium_and_above'.
        /// </summary>
        public string? SafetySetting { get; set; }

        /// <summary>
        /// 'dont_allow','allow_adult', 'allow_all'.
        /// Default: 'allow_adult'.
        /// </summary>
        public string? PersonGeneration { get; set; }

        public bool? IncludeRaiReason { get; set; }
        //public bool? IncludeSafetyAttributes { get; set; }

        #endregion
    }

    public class ImagenGenerateImageOutputOptions
    {
        /// <summary>
        /// 'image/png', 'image/jpeg'.
        /// Default: 'image/png'.
        /// </summary>
        public string? MimeType { get; set; }

        public int? CompressionQuality { get; set; }
    }
}
