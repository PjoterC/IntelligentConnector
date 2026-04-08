using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntelligentConnector.Core.Interfaces;
using IntelligentConnector.Core.Models;
using IntelligentConnector.Data;

namespace IntelligentConnector.Endpoints;

public static class CatEndpoints
{
    
}

public static class SyncEndpoints
{
    // public static void MapSyncEndpoints(this IEndpointRouteBuilder app)
    // {
    //     var group = app.MapGroup("/sync");

    //     // POST /sync - Triggers the process of fetching and updating data
    //     group.MapPost("/", async ([FromServices] IPublicApiConnector connector, [FromServices] AppDbContext db) =>
    //     {
    //         try 
    //         {
    //             // 1. Fetching data from the external API
    //             var dataFromApi = await connector.GetExternalDataAsync();

    //             if (dataFromApi == null || dataFromApi.Count == 0)
    //             {
    //                 return Results.NotFound("No data found.");
    //             }

    //             // 2. Retrieving all IDs from the incoming data
    //             var incomingExternalIds = dataFromApi.Select(a => a.id).ToList();

    //             // 3. Retrieving existing records from the database for comparison (Upsert)
    //             var existingRecords = await db.RestfulApiRecords
    //                 .Where(r => incomingExternalIds.Contains(r.id))
    //                 .ToDictionaryAsync(r => r.id);

    //             int updatedCount = 0;
    //             int createdCount = 0;

    //             foreach (var apiRec in dataFromApi)
    //             {
    //                 if (existingRecords.TryGetValue(apiRec.id, out var existing))
    //                 {
    //                     // --- UPDATE ---
    //                     existing.name = apiRec.name;
    //                     existing.data = apiRec.data; 
    //                     existing.UpdatedAt = DateTime.UtcNow; 
                        
    //                     updatedCount++;
    //                 }
    //                 else
    //                 {
    //                     // --- INSERT ---
    //                     apiRec.UpdatedAt = DateTime.UtcNow;
    //                     db.RestfulApiRecords.Add(apiRec);
    //                     createdCount++;
    //                 }
    //             }

    //             // 4. Saving all changes in a single transaction
    //             await db.SaveChangesAsync();

    //             return Results.Ok(new 
    //             { 
    //                 Status = "Success", 
    //                 TotalFetched = dataFromApi.Count, 
    //                 Updated = updatedCount, 
    //                 Created = createdCount,
    //                 Timestamp = DateTime.UtcNow 
    //             });
    //         }
    //         catch (Exception ex)
    //         {
    //             // Logging the error (consider adding ILogger in the parameters if needed)
    //             return Results.Problem($"An error occurred during synchronization: {ex.Message}");
    //         }
    //     })
    //     .WithName("TriggerSync");

    //     // GET /sync/data - Allows quickly previewing what's in the database
    //     group.MapGet("/data", async ([FromServices] AppDbContext db) => 
    //     {
    //         var records = await db.RestfulApiRecords.OrderByDescending(r => r.id).Take(100).ToListAsync();
    //         return Results.Ok(records);
    //     })
    //     .WithName("GetStoredData");
    // }
}