using IntelligentConnector.Core.Models;

namespace IntelligentConnector.Core.Interfaces;

public interface IPublicApiConnector
{
    Task<CatFact> GetCatFactAsync();
    Task<CatImage> GetCatImageAsync(CatFact fact);
}