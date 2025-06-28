using OCRService.Config;
using OCRService.Domain.Entities.Input;
using OCRService.Domain.Entities.Output;
using OCRService.Domain.Interfaces;
using OCRService.Infrastructure.OCR.DTOs;
using SharedKernel.Base;
using System.Text;
using System.Text.Json;

namespace OCRService.Infrastructure.OCR;

public class OCRServiceProvider : IOCRServiceProvider
{
    private readonly Uri _ocrServiceApiBaseUri = CONFIG.OCRServiceBaseUri;
    private String _ocrServiceApiKey = CONFIG.OCRServiceApiKey;
    private Error _serviceError = new ("OCR.SERVICE_ERROR",
        "An error occurred while processing OCR request, please try again later",
        ErrorType.Problem);

    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    public OCRServiceProvider()
    {
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

    public async Task<Result<OCROutput>> ProcessOCR(OCRInput ocrInput)
    {
        #region Build payload
        var payload = OCRInputDTO.FromDomain(ocrInput);
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        #endregion

        #region Call upstream
        HttpResponseMessage response;
        try
        {
            response = await _http.PostAsync("ocr/predict", content);
        }
        catch (Exception)
        {
            return Result.Failure<OCROutput>(_serviceError);
        }

        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<OCROutput>(_serviceError);
        }
        #endregion

        #region Deserialize and get result
        try
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<OCROutputDTO>(stream, _jsonOptions);
            return Result.Success(result!.ToDomain())!;
        }
        catch (Exception)
        {
            return Result.Failure<OCROutput>(_serviceError);
        }
        #endregion
    }
}
