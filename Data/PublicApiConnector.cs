using System.Net;
using System.Net.Http.Json;
using IntelligentConnector.Core.Interfaces;
using IntelligentConnector.Core.Models;

namespace IntelligentConnector.Data;

public class PublicApiConnector(IHttpClientFactory httpClientFactory) : IPublicApiConnector
{
    private const string CatFactsClientName = "CatFactsApi";
    private const string CatImagesClientName = "CatImagesApi";

    public async Task<CatFact> GetCatFactAsync()
    {
        var client = httpClientFactory.CreateClient(CatFactsClientName);

        try
        {
            using var response = await client.GetAsync("/api/v2/facts/random");
            if (!response.IsSuccessStatusCode)
            {
                LogRequestFailure(response.StatusCode, response.ReasonPhrase);
                return new CatFact(" ", "I failed to fetch your fact. Maybe the cat is sleeping on the API server?", "p4wVprNdce0EzbGl");
            }

            return await response.Content.ReadFromJsonAsync<CatFact>() ?? new CatFact(" ", "I failed to fetch your fact. Maybe the cat is sleeping on the API server?", "p4wVprNdce0EzbGl");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while fetching cat fact: {ex.Message}");
            return new CatFact(" ", "I failed to fetch your fact. Maybe the cat is sleeping on the API server?", "p4wVprNdce0EzbGl");
        }
    }
//
    public async Task<CatImageData> GetCatImageDataAsync()
    {
        var client = httpClientFactory.CreateClient(CatImagesClientName);

        try
        {
            using var response = await client.GetAsync("/cat?json=true");
            if (!response.IsSuccessStatusCode)
            {
                LogRequestFailure(response.StatusCode, response.ReasonPhrase);
                return new CatImageData { Id = string.Empty };
            }

            var data = await response.Content.ReadFromJsonAsync<CatImageData>();
            return new CatImageData { Id = data?.Id ?? string.Empty };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while fetching cat image data: {ex.Message}");
            return new CatImageData { Id = string.Empty };

        }
    }


    public async Task<CatImage> GetCatImageAsync(CatFact fact, CatImageData imageData)
    {
        var client = httpClientFactory.CreateClient(CatImagesClientName);

        //hello?fontSize=50&fontColor=white

        // Todo: Adjust font size better
        int length = fact.Length;
        var fontSize = length > 40 ? 25 : 50;
        

        string requestUrl = $"cat/{imageData.Id}/says/{Uri.EscapeDataString(fact.Text)}?fontSize={fontSize}&fontColor=white";
        try
        {
            var imageStream = await client.GetStreamAsync(requestUrl);
            return new CatImage { ImageStream = imageStream };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while fetching cat image: {ex.Message}");
            return new CatImage();
        }
    }

    private static void LogRequestFailure(HttpStatusCode statusCode, string? reasonPhrase)
    {
        Console.WriteLine($"API request failed: {(int)statusCode} {reasonPhrase}");
        if (statusCode == HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("Unauthorized access to external API. Check API key or credentials.");
        }
    }
}