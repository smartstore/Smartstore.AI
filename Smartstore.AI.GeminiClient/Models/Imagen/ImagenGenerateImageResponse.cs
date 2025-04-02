#nullable enable

namespace Smartstore.AI.GeminiClient
{
    public class ImagenGenerateImageResponse
    {
        public List<ImagenGenerateImagePrediction> Predictions { get; set; } = [];

        public override string ToString()
            => string.Join(Environment.NewLine, Predictions?.Select(x => x.ToString()) ?? []);
    }

    public class ImagenGenerateImagePrediction
    {
        public string? BytesBase64Encoded { get; set; }

        public string? MimeType { get; set; }

        public string? Prompt { get; set; }

        public string? RaiFilteredReason { get; set; }

        public override string ToString()
            => $"mimeType:{MimeType} bytesBase64Encoded:{BytesBase64Encoded?[..40]?.Trim() + '…'}";
    }

    //public class ImagenGenerateImageSafetyAttribute
    //{
    //    //categories,scores
    //}
}
