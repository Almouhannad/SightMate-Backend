using SharedKernel.Base;
using SharedKernel.Multimedia;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using VQAService.Config;
using VQAService.Domain.Entities.Conversations;
using VQAService.Domain.Interfaces;
using VQAService.Infrastructure.VQA.DTOs;

namespace VQAService.Infrastructure.VQA;

public class VQAServiceProvider : IVQAServiceProvider
{
    private readonly Uri _ocrServiceApiBaseUri = CONFIG.VqaServiceBaseUri;
    private readonly String _ocrServiceApiKey = CONFIG.VqaServiceApiKey;
    private readonly Error _serviceError = new("VQA.SERVICE_ERROR",
        "An error occurred while processing VQA request, please try again later",
        ErrorType.Conflict);

    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    public VQAServiceProvider(HttpClient httpClient)
    {
        _http = httpClient;
        _http = new HttpClient { BaseAddress = _ocrServiceApiBaseUri };
        _http.DefaultRequestHeaders.Add("X-API-Key", _ocrServiceApiKey);
    }

    public async Task<bool> IsAvailable()
    {
        try
        {
            using var resp = await _http.GetAsync("health");
            if (!resp.IsSuccessStatusCode)
                return false;

            var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
            return doc.RootElement.GetProperty("status").GetString()?.Equals("ok", StringComparison.OrdinalIgnoreCase) == true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Result<string>> ProcessIC(Image image)
    {
        try
        {
            // 1. Build request DTO
            var req = new CaptioningRequestDTO
            {
                Image = image.ToDTO(),
                History = null
            };

            // 2. POST to /vqa/captioning
            var resp = await _http.PostAsJsonAsync(
                "vqa/captioning",
                req,
                _jsonOptions);

            // 3. Handle non-success
            if (resp.StatusCode != HttpStatusCode.OK)
                return Result.Failure<string>(_serviceError);

            // 4. Deserialize and map
            var payload = await resp.Content.ReadFromJsonAsync<ResponseDTO>(_jsonOptions);

            return payload!.ToDomain();
        }
        catch (Exception)
        {
            // TODO: LOG
            return Result.Failure<string>(_serviceError);
        }
    }

    public async Task<Result<string>> ProcessVQA(Image image, string question, List<HistoryItem>? history)
    {
        try
        {
            // 1. Build request DTO
            var req = new QuestionRequestDTO
            {
                Image = image.ToDTO(),
                Question = question,
                History = history?.Select(h => h.ToDTO()).ToList()
            };

            // 2. POST to /vqa/question
            var resp = await _http.PostAsJsonAsync(
                "vqa/question",
                req,
                _jsonOptions);

            // 3. Handle non-success
            if (resp.StatusCode != HttpStatusCode.OK)
                return Result.Failure<string>(_serviceError);

            // 4. Deserialize and map
            var payload = await resp.Content.ReadFromJsonAsync<ResponseDTO>(_jsonOptions);

            return payload!.ToDomain();
        }
        catch (Exception)
        {
            return Result.Failure<string>(_serviceError);
        }
    }
}
