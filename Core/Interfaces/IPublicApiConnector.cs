using IntelligentConnector.Core.Models;

namespace IntelligentConnector.Core.Interfaces;

public interface IPublicApiConnector
{
    Task<CatFact> GetExternalDataAsync();
    Task<CatImage> GetExternalImageAsync();
}