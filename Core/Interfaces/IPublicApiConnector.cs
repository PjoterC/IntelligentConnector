using IntelligentConnector.Core.Models;

namespace IntelligentConnector.Core.Interfaces;

public interface IPublicApiConnector
{
    Task<CatFact> GetCatFactAsync();

    Task<CatImageData> GetCatImageDataAsync();
    Task<CatImage> GetCatImageAsync(CatFact fact, CatImageData imageData);

    
}