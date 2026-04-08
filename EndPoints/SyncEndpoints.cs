using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntelligentConnector.Core.Interfaces;
using IntelligentConnector.Core.Models;
using IntelligentConnector.Data;


namespace IntelligentConnector.Endpoints;

public static class CatEndpoints
{
    public static void MapCatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/cat");

        group.MapGet("/newfact", async ([FromServices] IPublicApiConnector connector,  [FromServices] AppDbContext db) =>
        {
            var existingRecords = await db.CatFacts.ToListAsync();
            var existingIDs = existingRecords.Select(r => r.Id).ToHashSet();

            var fact = await connector.GetCatFactAsync();
            int attempts = 0;
            
            const int maxAttempts = 3;
            while (existingIDs.Contains(fact.Id))
            {
                fact = await connector.GetCatFactAsync();
                if (++attempts >= maxAttempts)
                {
                    db.CatFacts.RemoveRange(existingRecords);
                    await db.SaveChangesAsync();
                    fact.Text = "You wanted to remember more facts than a mere mortal can handle, so I purged your memory.";
                    var img = await connector.GetCatImageAsync(fact);
                    return Results.File(img.ImageStream, "image/png");
                    
                }
                
            }
            fact.Length = fact.Text.Length;
            db.CatFacts.Add(fact);
            await db.SaveChangesAsync();

            var image = await connector.GetCatImageAsync(fact);
            return Results.File(image.ImageStream, "image/png");


        })
        .WithName("GetCatFact");

        group.MapGet("/rememberfact", async ([FromServices] IPublicApiConnector connector, [FromServices] AppDbContext db) =>
        {
            var existingRecords = await db.CatFacts.ToListAsync();
            if (!existingRecords.Any())
            {
                var fact = new CatFact("0", "You haven't asked for any cat facts yet, so I have nothing to remember.");
                var img = await connector.GetCatImageAsync(fact);
                return Results.File(img.ImageStream, "image/png");
            }
            var random = new Random();
            var factToRemember = existingRecords[random.Next(existingRecords.Count)];
            var image = await connector.GetCatImageAsync(factToRemember);
            return Results.File(image.ImageStream, "image/png");
        }).WithName("RememberCatFact");


        

        
    }
}

// public static class SyncEndpoints
// {
//     public static void MapSyncEndpoints(this IEndpointRouteBuilder app)
//     {
//         var group = app.MapGroup("/sync");

//         group.MapGet("/fact", async ([FromServices] IPublicApiConnector connector) =>
//         {
//             var fact = await connector.GetCatFactAsync();
//             return Results.Ok(fact);
//         })
//         .WithName("GetCatFact");

//         group.MapGet("/image", async ([FromServices] IPublicApiConnector connector) =>
//         {
//             var image = await connector.GetCatImageAsync();
//             return Results.Ok(image);
//         })
//         .WithName("GetCatImage");

//         group.MapPost("/save", async ([FromServices] IPublicApiConnector connector, [FromServices] AppDbContext db) =>
//         {
//             var fact = await connector.GetCatFactAsync();
//             var image = await connector.GetCatImageAsync();

//             db.CatFacts.Add(fact);
//             db.CatImages.Add(image);
//             await db.SaveChangesAsync();

//             return Results.Ok(new
//             {
//                 Message = "Cat fact and image saved.",
//                 Fact = fact,
//                 Image = image
//             });
//         })
//         .WithName("SyncAndSave");
//     }
// }