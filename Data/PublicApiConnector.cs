using System.Net;
using System.Net.Http.Json;
using IntelligentConnector.Core.Models;
using IntelligentConnector.Core.Interfaces;
using System.Diagnostics;
using System.Text.Json;

namespace IntelligentConnector.Data;
public class PublicApiConnector(HttpClient httpClient) : IPublicApiConnector
{
    public async Task<CatFact> GetExternalDataAsync()
    {
        try
        {
            using var response = await httpClient.GetAsync("objects");
                   
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API request failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Unauthorized access to external API. Check API key or credentials.");
                }
                return new List<RestfulApiRecord>();
                
            }
            var content = await response.Content.ReadAsStringAsync();
            //Console.WriteLine($"Response content: {content}");
            var jsonDoc = JsonDocument.Parse(content);
            if (jsonDoc.RootElement.ValueKind == JsonValueKind.Object)
            {
                Console.WriteLine("Received JSON object instead of array. Wrapping in array.");
                var wrappedContent = $"[{content}]";
                return JsonSerializer.Deserialize<List<RestfulApiRecord>>(wrappedContent) ?? new List<RestfulApiRecord>();
            }
            return await response.Content.ReadFromJsonAsync<List<RestfulApiRecord>>() ?? new List<RestfulApiRecord>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request error: {ex.Message}");
            return new List<RestfulApiRecord>();
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Request timed out: {ex.Message}");
            return new List<RestfulApiRecord>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON parsing error: {ex.Message}");
            return new List<RestfulApiRecord>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return new List<RestfulApiRecord>();
        }
    }
}