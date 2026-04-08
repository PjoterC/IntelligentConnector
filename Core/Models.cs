using System.Text.Json;

namespace IntelligentConnector.Core.Models;

public class PublicApiResponse
{
    public int Count { get; set; }
    public List<ApiEntry> Entries { get; set; } = new();
}

public class ApiEntry
{
    public string Api { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsHttps { get; set; }
}



public class CatFact
{
    public CatFact(string id, string text, string catImageId)
    {
        Id = id;
        Text = text;
        Length = text.Length;
        CatImageId = catImageId;
    }
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Length { get; set; }

    public string CatImageId { get; set; } = string.Empty; 
}

public class CatImageData
{
    public string Id { get; set; } = string.Empty;


}

public class CatImage
{
    public Stream ImageStream { get; set; } = Stream.Null;
}