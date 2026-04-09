using System.Net;
using IntelligentConnector.Core.Interfaces;
using IntelligentConnector.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
namespace IntelligentConnector.Data;

public class PublicApiConnector(IHttpClientFactory httpClientFactory) : IPublicApiConnector
{
    private const string CatFactsClientName = "CatFactsApi";
    private const string CatImagesClientName = "CatImagesApi";

    // returns cat fact
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
    // Returns cat image json data
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

    //returns cat image with the fact text drawn on it
    public async Task<CatImage> GetCatImageAsync(CatFact fact, CatImageData imageData)
{
    var client = httpClientFactory.CreateClient(CatImagesClientName);

    var bytes = await client.GetByteArrayAsync($"cat/{imageData.Id}");

    

    string requestUrl = $"cat/{imageData.Id}";
    try
    {
        // Read the image as a byte array and return it as a stream to avoid issues with content type and encoding
        var response = await client.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        
   
        var finalStream = await ModifyImageLocally(await response.Content.ReadAsStreamAsync(), fact.Text);
        return new CatImage { ImageStream = finalStream };
    }
    catch (Exception ex)
    {
        Console.WriteLine($"API Error, returning fallback image: {ex.Message}");

        // Path to the local fallback file
        string fallbackPath = Path.Combine(AppContext.BaseDirectory, "apidown.jpg");

        if (File.Exists(fallbackPath))
        {
            var fallbackStream = File.OpenRead(fallbackPath);
            return new CatImage { ImageStream = fallbackStream };
        }

        // If even the file doesn't exist, return an empty stream (last resort)
        return new CatImage { ImageStream = Stream.Null };
    }
}

// For font formatting
public async Task<Stream> ModifyImageLocally(Stream imageStream, string text)
{
    using var image = await Image.LoadAsync(imageStream);
    
    
    var font = SystemFonts.CreateFont("Impact", image.Width / 12);

    image.Mutate(x => {
        var options = new RichTextOptions(font) {
            Origin = new PointF(image.Width / 2f, image.Height - 50f), 
            WrappingLength = image.Width - 40,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom
        };
        
        
        x.DrawText(options, text, Color.White);
    });

    var output = new MemoryStream();
    await image.SaveAsPngAsync(output);
    output.Position = 0;
    return output;
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