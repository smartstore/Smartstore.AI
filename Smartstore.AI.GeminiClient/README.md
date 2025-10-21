## AI Gemini client

![NuGet Version](https://img.shields.io/nuget/v/Smartstore.AI.GeminiClient) ![NuGet Downloads](https://img.shields.io/nuget/dt/Smartstore.AI.GeminiClient)

This project provides a .NET client library designed to facilitate interaction with the Gemini API. 
It offers developers a streamlined approach to integrating Gemini's powerful AI capabilities into their .NET applications.
Key features include:

- Simplified API calls: Abstracts the complexities of direct API interaction.
- Data handling: Efficiently manages request and response data.
- Flexibility: Adaptable to various .NET environments.

## Implemented features 
- [x] Generate text or image content (including image input).
- [x] Generate content as stream.
- [x] Generate images using [Imagen](https://deepmind.google/models/imagen/).
- [x] File operations: Upload, get file list, delete file.
- [x] Get list of available Gemini models.

## Quickstart
Get a [Google Gemini API key](https://ai.google.dev/gemini-api/docs/api-key).
To install the Smartstore.AI.GeminiClient NuGet package, run the following command in the NuGet Package Manager Console:

```sh
Install-Package Smartstore.AI.GeminiClient
```

Or, if you prefer using the .NET CLI:

```sh
dotnet add package Smartstore.AI.GeminiClient
```

## Usage
You can seamlessly integrate the `GeminiAIClient` into your code using dependency injection.

```csharp

// "services" is of type IServiceCollection.
services.AddHttpClient<GeminiAIClient>()
	.ConfigureHttpClient(client =>
	{
		client.Timeout = TimeSpan.FromSeconds(30);
	});
```

Alternatively, you can instantiate the `GeminiAIClient` class manually. To do this, simply pass an `HttpClient` instance to the constructor.

```csharp
using System.Net.Http;
using Smartstore.AI.GeminiClient;

public class MyHttpClass
{
	private readonly IHttpClientFactory _httpClientFactory;

	public MyHttpClass(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public void MyHttpMethod()
	{
		var client = new GeminiAIClient(_httpClientFactory.CreateClient());
		//...
	}
}
```

## Configuration
The `GeminiConfig` class is used to provide `GeminiAIClient` with the data required for communication with Gemini:

```csharp
var config = new GeminiConfig(
	"Your Google Gemini API key", 
	"AI model name", 
	"Gemini API base URL (optional)");
```

A method call of the client is always made by passing an instance of `GeminiConfig`.

## Examples
### Generate text

```csharp
public class GeminiProvider(GeminiAIClient client)
{
	private readonly GeminiAIClient _client = client;
	
	public async Task<string> ChatAsync(
		string message /*e.g. How does AI work?*/,
		CancellationToken cancelToken = default)
	{
		var config = new GeminiConfig("my-api-key...", "gemini-2.0-flash");
		var contents = new List<GeminiContent>
		{
			new GeminiContent
			{
				Role = "user",
				Parts = [new GeminiPart
				{
					Text = message
				}]
			}
		};

		// Give the AI model additional context to help it understand the task
		// or follow specific guidelines (optional).
		var instructions = new List<GeminiPart>
		{
			new GeminiPart
			{
				Text = "Please answer in a friendly tone."
			}
		};

		// Control how the AI model generates responses (optional).
		var generationConfig = new GeminiGenerationConfig
		{
			Temperature = 0.8f,
			MaxOutputTokens = 500
		};

		var request = new GeminiGenerateContentRequest
		{
			Contents = contents,
			SystemInstruction = new GeminiContent { Parts = instructions },
			GenerationConfig = generationConfig
		};

		var response = await _client.GenerateContentAsync(config, request, cancelToken);
		var candidate = response.Candidates.FirstOrDefault();
		
		return candidate?.Content?.Parts?.FirstOrDefault()?.Text;	
	}
}
```

### Streaming output
```csharp
public class GeminiProvider(GeminiAIClient client)
{
	private readonly GeminiAIClient _client = client;
	
	public async IAsyncEnumerable<string> ChatAsStreamAsync(
		string message /*e.g. How does AI work?*/,
		[EnumeratorCancellation] CancellationToken cancelToken = default)
	{
		var config = new GeminiConfig("my-api-key...", "gemini-2.0-flash");
		var contents = new List<GeminiContent>
		{
			new GeminiContent
			{
				Role = "user",
				Parts = [new GeminiPart
				{
					Text = message
				}]
			}
		};

		// Give the AI model additional context to help it understand the task
		// or follow specific guidelines (optional).
		var instructions = new List<GeminiPart>
		{
			new GeminiPart
			{
				Text = "Please answer in a friendly tone."
			}
		};

		var request = new GeminiGenerateContentRequest
		{
			Contents = contents,
			SystemInstruction = new GeminiContent { Parts = instructions }
		};

		await foreach (var response in _client.GenerateContentAsStreamAsync(config, request, cancelToken))
		{
			var candidate = response.Candidates.FirstOrDefault();
			var answer = candidate?.Content?.Parts?.FirstOrDefault()?.Text;
			if (answer != null)
			{
				yield return answer;
			}
		}
	}
}
```
