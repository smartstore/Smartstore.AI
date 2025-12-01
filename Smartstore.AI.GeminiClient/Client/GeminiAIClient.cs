using Microsoft.Extensions.FileProviders;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Smartstore.AI.GeminiClient
{
    public partial class GeminiAIClient(HttpClient httpClient)
    {
        const string DataPrefix = "data: ";
        const string StreamDoneSign = "[DONE]";
        const string BaseUrl = "https://generativelanguage.googleapis.com/{0}v1beta/{1}";

        protected static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public readonly HttpClient HttpClient = httpClient;

        /// <summary>
        /// Gets the Gemini's default output token limit.
        /// </summary>
        public static int DefaultOutputTokenLimit => 8192;

        #region Content generation

        /// <summary>
        /// Generates content like text output.
        /// </summary>
        /// <param name="config">API configuration data.</param>
        /// <param name="request">Generate content request.</param>
        /// <exception cref="HttpRequestException"></exception>
        public virtual async Task<GeminiGenerateContentResponse> GenerateContentAsync(
            GeminiConfig config,
            GeminiGenerateContentRequest request,
            CancellationToken cancelToken = default)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(request);

            var json = JsonSerializer.Serialize(request, SerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            var url = $"{CreateBaseUrl(config, "models")}{config.ModelName}:generateContent?key={config.ApiKey}";

            using var message = await HttpClient.PostAsync(url, content, cancelToken);
            await EnsureSuccess(json, message, cancelToken);

            var rawContent = await message.Content.ReadAsStringAsync(cancelToken);
            var response = JsonSerializer.Deserialize<GeminiGenerateContentResponse>(rawContent, SerializerOptions);

            if (response?.Candidates == null)
            {
                throw CreateException("The content candidates are missing from the Gemini response.", json, rawContent);
            }

            return response;
        }

        /// <summary>
        /// Generates a text stream that returns partial results of the AI answer.
        /// Only applicable to generate text from a text-only input prompt.
        /// </summary>
        /// <param name="config">API configuration data.</param>
        /// <param name="request">Generate content request.</param>
        /// <exception cref="HttpRequestException"></exception>
        public virtual async IAsyncEnumerable<GeminiGenerateContentResponse> GenerateContentAsStreamAsync(
            GeminiConfig config,
            GeminiGenerateContentRequest request,
            [EnumeratorCancellation] CancellationToken cancelToken = default)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(request);

            var json = JsonSerializer.Serialize(request, SerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            // INFO sse: A value indicating whether to use server-side events.
            // With SSE each stream chunk is a GenerateContentResponse object with a portion of the output text in candidates[0].content.parts[0].text.
            var url = $"{CreateBaseUrl(config, "models")}{config.ModelName}:streamGenerateContent?key={config.ApiKey}&alt=sse";

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            using var message = await HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancelToken);
            await EnsureSuccess(json, message, cancelToken);

            var stream = await message.Content.ReadAsStreamAsync(cancelToken);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream && !cancelToken.IsCancellationRequested)
            {
                var contentResponse = await GetResponse(reader, cancelToken);
                if (contentResponse != null)
                {
                    yield return contentResponse;
                }
            }
        }

        private async static Task<GeminiGenerateContentResponse?> GetResponse(StreamReader reader, CancellationToken cancelToken)
        {
#if NET8_0
                var rawData = await reader.ReadLineAsync(cancelToken);
                if (!string.IsNullOrEmpty(rawData) && rawData.StartsWith(DataPrefix))
                {
                    var rawJson = rawData[6..];
                    if (!string.IsNullOrEmpty(rawJson) && rawJson != StreamDoneSign)
                    {
                        return JsonSerializer.Deserialize<GeminiGenerateContentResponse>(rawJson, SerializerOptions);
                    }
                }
#else
            // INFO: Use Span<T> instead of string for performance reasons in hot path codes, especially in "fast" loops, to reduce mem allocations.
            var rawData = (await reader.ReadLineAsync(cancelToken)).AsSpan();
            if (!rawData.IsEmpty && rawData.StartsWith(DataPrefix))
            {
                var rawJson = rawData[6..];
                if (!rawJson.IsEmpty && !rawJson.SequenceEqual(StreamDoneSign))
                {
                    return JsonSerializer.Deserialize<GeminiGenerateContentResponse>(rawJson, SerializerOptions);
                }
            }
#endif
            return null;
        }

        #endregion

        #region Imagen

        /// <summary>
        /// Generates images using Imagen.
        /// </summary>
        /// <param name="config">API configuration data.</param>
        /// <exception cref="HttpRequestException"></exception>
        /// <remarks>
        /// Gemini generates inline, contextual images only in combination with output text,
        /// e.g. a blog post with text and images in a single turn.
        /// Imagen, on the other hand, produces the highest quality text-to-image output.
        /// </remarks>
        public virtual async Task<ImagenGenerateImageResponse> GenerateImagenImagesAsync(
            GeminiConfig config,
            ImagenGenerateImageRequest request,
            CancellationToken cancelToken = default)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(request);

            var json = JsonSerializer.Serialize(request, SerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            var url = $"{CreateBaseUrl(config, "models")}{config.ModelName}:predict?key={config.ApiKey}";

            using var message = await HttpClient.PostAsync(url, content, cancelToken);
            await EnsureSuccess(json, message, cancelToken);

            var rawContent = await message.Content.ReadAsStringAsync(cancelToken);
            var response = JsonSerializer.Deserialize<ImagenGenerateImageResponse>(rawContent, SerializerOptions);

            if (response?.Predictions == null)
            {
                throw CreateException("The image data (prediction) is missing from the Gemini response.", json, rawContent);
            }

            return response;
        }

        #endregion

        #region File management

        /// <summary>
        /// Uploads a file to Gemini.
        /// </summary>
        /// <param name="config">API configuration data.</param>
        /// <param name="file">File to upload.</param>
        /// <param name="mimeType">Mime type of the file to upload. Required.</param>
        /// <exception cref="HttpRequestException"></exception>
        /// <see cref="https://ai.google.dev/gemini-api/docs/vision?lang=rest#upload-image"/>
        public virtual async Task<GeminiUploadFile> UploadFileAsync(
            GeminiConfig config,
            IFileInfo file,
            string mimeType,
            CancellationToken cancelToken = default)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(file);
            ArgumentException.ThrowIfNullOrEmpty(mimeType);

            var url = $"{CreateBaseUrl(config, "files", "upload")}?key={config.ApiKey}";
            var json = JsonSerializer.Serialize(new GeminiUploadFile
            {
                File = new()
                {
                    DisplayName = file.Name
                }
            }, SerializerOptions);

            var initRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            initRequest.Headers.Add("X-Goog-Upload-Protocol", "resumable");
            initRequest.Headers.Add("X-Goog-Upload-Command", "start");
            initRequest.Headers.Add("X-Goog-Upload-Header-Content-Length", file.Length.ToString());
            initRequest.Headers.Add("X-Goog-Upload-Header-Content-Type", mimeType);

            using var initiateResponse = await HttpClient.SendAsync(initRequest, cancelToken);
            await EnsureSuccess(json, initiateResponse, cancelToken);

            // Upload file.
            var uploadUrl = initiateResponse.Headers.GetValues("X-Goog-Upload-URL").FirstOrDefault();
            if (string.IsNullOrWhiteSpace(uploadUrl))
            {
                throw CreateException("Upload URL (X-Goog-Upload-URL) is missing from the Gemini response.", json, initiateResponse.Headers.ToString());
            }

            using var fileStream = file.CreateReadStream();
            var uploadRequest = new HttpRequestMessage(HttpMethod.Post, uploadUrl)
            {
                Content = new StreamContent(fileStream)
            };
            uploadRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            uploadRequest.Content.Headers.ContentLength = fileStream.Length;
            uploadRequest.Headers.Add("X-Goog-Upload-Command", "upload, finalize");
            uploadRequest.Headers.Add("X-Goog-Upload-Offset", "0");

            using var uploadResponse = await HttpClient.SendAsync(uploadRequest, cancelToken);
            await EnsureSuccess(null, uploadResponse, cancelToken);

            var rawContent = await uploadResponse.Content.ReadAsStringAsync(cancelToken);
            var uploadFile = JsonSerializer.Deserialize<GeminiUploadFile>(rawContent, SerializerOptions);

            if (uploadFile?.File == null || string.IsNullOrWhiteSpace(uploadFile.File.Uri))
            {
                throw CreateException("The URI of the uploaded file is missing from the Gemini response.", null, rawContent);
            }

            return uploadFile;
        }

        /// <summary>
        /// Gets a list of uploaded files.
        /// </summary>
        /// <param name="config">API configuration data.</param>
        /// <exception cref="HttpRequestException"></exception>
        public virtual async Task<GeminiFiles> GetFilesAsync(GeminiConfig config, CancellationToken cancelToken = default)
        {
            ArgumentNullException.ThrowIfNull(config);

            var url = $"{CreateBaseUrl(config, "files")}?key={config.ApiKey}";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await HttpClient.SendAsync(request, cancelToken);
            await EnsureSuccess(null, response, cancelToken);

            var rawContent = await response.Content.ReadAsStringAsync(cancelToken);
            var files = JsonSerializer.Deserialize<GeminiFiles>(rawContent, SerializerOptions);

            if (files?.Files == null)
            {
                throw CreateException("The list of uploaded files is missing from the Gemini response.", null, rawContent);
            }

            return files;
        }

        /// <summary>
        /// Deletes a Gemini hosted file.
        /// </summary>
        /// <remarks>Files are automatically deleted after 48 hours.</remarks>
        /// <param name="config">API configuration data.</param>
        /// <param name="fileNameOrUrl">Unique file name/ID (e.g. files/t1lue3159oa9) or Gemini file URL.</param>
        /// <exception cref="HttpRequestException"></exception>
        public virtual async Task DeleteFileAsync(
            GeminiConfig config,
            string fileNameOrUrl,
            CancellationToken cancelToken = default)
        {
            ArgumentNullException.ThrowIfNull(config);

            if (!string.IsNullOrWhiteSpace(fileNameOrUrl))
            {
                var url = IsWebUrl(fileNameOrUrl) ? fileNameOrUrl : $"{CreateBaseUrl(config)}{fileNameOrUrl}";
                url += (url.Contains('?') ? '&' : '?') + $"key={config.ApiKey}";

                var request = new HttpRequestMessage(HttpMethod.Delete, url);
                using var response = await HttpClient.SendAsync(request, cancelToken);
                await EnsureSuccess(null, response, cancelToken);
            }
        }

        #endregion

        #region Misc

        /// <summary>
        /// Gets a list of available models.
        /// </summary>
        /// <param name="config">API configuration data.</param>
        /// <param name="pageSize">
        /// The maximum number of Models to return (per page). If unspecified, 50 models will be returned per page.
        /// This method returns at most 1000 models per page, even if you pass a larger <paramref name="pageSize"/>.
        /// </param>
        /// <param name="pageToken">
        /// A page token, received from a previous models.list call. Provide the <paramref name="pageToken"/> 
        /// returned by one request as an argument to the next request to retrieve the next page.
        /// </param>
        /// <exception cref="HttpRequestException"></exception>
        public virtual async Task<GeminiModels> GetModelsAsync(
            GeminiConfig config,
            int pageSize = 0,
            string? pageToken = null,
            CancellationToken cancelToken = default)
        {
            ArgumentNullException.ThrowIfNull(config);

            var url = $"{CreateBaseUrl(config, "models")}?key={config.ApiKey}";
            if (pageSize > 0)
            {
                url += $"&pageSize={pageSize}";
            }
            if (!string.IsNullOrWhiteSpace(pageToken))
            {
                url += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await HttpClient.SendAsync(request, cancelToken);
            await EnsureSuccess(null, response, cancelToken);

            var rawContent = await response.Content.ReadAsStringAsync(cancelToken);
            var models = JsonSerializer.Deserialize<GeminiModels>(rawContent, SerializerOptions);

            if (models?.Models == null)
            {
                throw CreateException("The list of models is missing from the Gemini response.", null, rawContent);
            }

            return models;
        }

        #endregion

        #region Utilities

        protected static string CreateBaseUrl(GeminiConfig config, string? entity = null, string? method = null)
        {
            method ??= string.Empty;
            entity ??= string.Empty;

            return string.Format(CultureInfo.InvariantCulture, config.BaseUrl ?? BaseUrl,
                method.EndsWith('/') ? method : (method + '/'),
                entity.EndsWith('/') ? entity : (entity + '/'));
        }

        protected static async Task EnsureSuccess(string? requestJson, HttpResponseMessage response, CancellationToken cancelToken)
        {
            if (!response.IsSuccessStatusCode)
            {
                string? message = null;
                var responseJson = await response.Content.ReadAsStringAsync(cancelToken);

                if (!string.IsNullOrWhiteSpace(responseJson))
                {
                    try
                    {
                        message = JsonSerializer.Deserialize<GeminiErrorResponse>(responseJson, SerializerOptions)?.ToString();
                    }
                    catch
                    {
                    }
                }

                throw CreateException(
                    string.IsNullOrWhiteSpace(message) ? $"Gemini error {(int)response.StatusCode} {response.ReasonPhrase}" : message,
                    requestJson,
                    responseJson,
                    response);
            }
        }

        protected static HttpRequestException CreateException(
            string? message, 
            string? requestJson, 
            string? responseJson, 
            HttpResponseMessage? response = null)
        {
            var innerEx = string.IsNullOrWhiteSpace(requestJson) && string.IsNullOrWhiteSpace(responseJson)
                ? null
                : new Exception((requestJson ?? string.Empty) + Environment.NewLine + Environment.NewLine + (responseJson ?? string.Empty));

            return new(message, innerEx, response?.StatusCode);
        }

        protected static bool IsWebUrl(string? value, bool schemeIsOptional = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            value = value.Trim().ToLowerInvariant();

            if (schemeIsOptional && value.StartsWith("//"))
            {
                value = "http:" + value;
            }

            return Uri.IsWellFormedUriString(value, UriKind.Absolute) &&
                (value.StartsWith("http://") || value.StartsWith("https://") || value.StartsWith("ftp://"));
        }

        #endregion
    }
}
