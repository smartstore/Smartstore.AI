#nullable enable

namespace Smartstore.AI.GeminiClient
{
    /// <summary>
    /// <see cref="https://ai.google.dev/api/files#method:-media.upload" />
    /// </summary>
    public class GeminiUploadFile
    {
        public GeminiFile? File { get; set; }
    }

    /// <summary>
    /// <see cref="https://ai.google.dev/api/files#method:-files.list" />
    /// </summary>
    public class GeminiFiles
    {
        public List<GeminiFile>? Files { get; set; }

        public override string ToString()
            => string.Join(Environment.NewLine, Files?.Select(x => x.ToString()) ?? []);
    }

    /// <summary>
    /// <see cref="https://ai.google.dev/api/files#File" />
    /// </summary>
    public class GeminiFile
    {
        /// <summary>
        /// The unique file name/ID.
        /// </summary>
        /// <example>files/t1lue3159oa9</example>
        public string? Name { get; set; }

        public string? DisplayName { get; set; }

        public string? MimeType { get; set; }

        public string? SizeBytes { get; set; }

        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? ExpirationTime { get; set; }

        public string? Sha256Hash { get; set; }

        /// <summary>
        /// The unique file URI.
        /// </summary>
        /// <example>https://generativelanguage.googleapis.com/v1beta/files/t1lue3159oa9</example>
        public string? Uri { get; set; }

        public string? DownloadUri { get; set; }

        /// <summary>
        /// <see cref="https://ai.google.dev/api/files#State" />
        /// </summary>
        /// <example>ACTIVE</example>
        public string? State { get; set; }

        /// <summary>
        /// <see cref="https://ai.google.dev/api/files#Source" />
        /// </summary>
        /// <example>UPLOADED</example>
        public string? Source { get; set; }

        public GeminiStatus? Error { get; set; }

        public override string ToString()
            =>  $"name:{Name} displayName:{DisplayName} createTime:{CreateTime}";
    }
}
