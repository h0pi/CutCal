using System.Globalization;
using System.Text.Json;
using CutCal.Model.Exceptions;
using CutCal.Model.Responses;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CutCal.Services.Services;

public interface IGeocodingService
{
    Task<List<GeocodeResultResponse>> SearchAsync(string query);
}

/// <summary>Turns a typed address into coordinates using OpenStreetMap Nominatim.</summary>
public class GeocodingService : IGeocodingService
{
    public const string HttpClientName = "geocoding";

    private const int MaxResults = 5;
    private const int MinQueryLength = 3;
    private const int MaxQueryLength = 200;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GeocodingService> _logger;

    public GeocodingService(IHttpClientFactory httpClientFactory, IMemoryCache cache, ILogger<GeocodingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<GeocodeResultResponse>> SearchAsync(string query)
    {
        var trimmed = query?.Trim() ?? string.Empty;
        if (trimmed.Length < MinQueryLength || trimmed.Length > MaxQueryLength)
        {
            throw new ClientException($"Enter an address between {MinQueryLength} and {MaxQueryLength} characters, e.g. \"Ferhadija 1, Sarajevo\".");
        }

        var cacheKey = $"geocode:{trimmed.ToLowerInvariant()}";
        if (_cache.TryGetValue(cacheKey, out List<GeocodeResultResponse>? cached) && cached is not null)
        {
            return cached;
        }

        var client = _httpClientFactory.CreateClient(HttpClientName);
        var url = $"search?format=json&limit={MaxResults}&q={Uri.EscapeDataString(trimmed)}";

        try
        {
            using var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);

            var results = document.RootElement.EnumerateArray()
                .Select(item => new GeocodeResultResponse
                {
                    DisplayName = item.GetProperty("display_name").GetString() ?? trimmed,
                    Latitude = double.Parse(item.GetProperty("lat").GetString()!, CultureInfo.InvariantCulture),
                    Longitude = double.Parse(item.GetProperty("lon").GetString()!, CultureInfo.InvariantCulture)
                })
                .ToList();

            _cache.Set(cacheKey, results, CacheDuration);
            return results;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or KeyNotFoundException)
        {
            _logger.LogError(ex, "Geocoding lookup failed for query '{Query}'", trimmed);
            throw new ClientException("Address lookup is temporarily unavailable. Please try again in a moment.");
        }
    }
}
