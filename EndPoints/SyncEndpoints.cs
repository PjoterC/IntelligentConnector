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
            
            var existingCatIDs = await db.CatImages.Select(i => i.Id).ToListAsync();
            var imageData = await connector.GetCatImageDataAsync();

            // Attempt to use a new cat
            int catAttempts = 0;
            const int maxCatAttempts = 5;
            while (existingCatIDs.Contains(imageData.Id) && catAttempts < maxCatAttempts)
            {
                imageData = await connector.GetCatImageDataAsync();
                catAttempts++;
            }

            if (!existingCatIDs.Contains(imageData.Id))
            {
                db.CatImages.Add(imageData);
                await db.SaveChangesAsync();
            }
            


            var fact = await connector.GetCatFactAsync();
            
            // Attempt to get a new fact if the ID already exists, up to a certain number of attempts
            int factAttempts = 0;
            const int maxFactAttempts = 3;
            while (existingIDs.Contains(fact.Id))
            {
                fact = await connector.GetCatFactAsync();
                if (++factAttempts >= maxFactAttempts)
                {
                    db.CatFacts.RemoveRange(existingRecords);
                    await db.SaveChangesAsync();
                    fact.Text = "You wanted to remember more facts than a mere mortal can handle, so I purged your memory.";
                    var img = await connector.GetCatImageAsync(fact,imageData);
                    return Results.File(img.ImageStream, "image/png");
                    
                }
                
            }
            
            fact.CatImageId = imageData.Id;
            db.CatFacts.Add(fact);
            await db.SaveChangesAsync();

            var image = await connector.GetCatImageAsync(fact, imageData);
            return Results.File(image.ImageStream, "image/png");


        })
        .WithName("GetCatFact");

        group.MapGet("/rememberfact", async ([FromServices] IPublicApiConnector connector, [FromServices] AppDbContext db) =>
        {
            var existingRecords = await db.CatFacts.ToListAsync();
            if (!existingRecords.Any())
            {
                var fact = new CatFact("0", "You haven't asked for any cat facts yet, so I have nothing to remember.", "p4wVprNdce0EzbGl");
                var imgData = await connector.GetCatImageDataAsync();
                var img = await connector.GetCatImageAsync(fact, imgData);
                return Results.File(img.ImageStream, "image/png");
            }
            var random = new Random();
            var factToRemember = existingRecords[random.Next(existingRecords.Count)];

            var imageData = new CatImageData { Id = factToRemember.CatImageId };
            var image = await connector.GetCatImageAsync(factToRemember, imageData);
            return Results.File(image.ImageStream, "image/png");
        }).WithName("RememberCatFact");


        group.MapDelete("/clearfacts", async ([FromServices] AppDbContext db) =>
        {
            var allFacts = await db.CatFacts.ToListAsync();
            db.CatFacts.RemoveRange(allFacts);
            await db.SaveChangesAsync();
            return Results.Ok("All cat facts cleared from memory.");
        }).WithName("ClearCatFacts");
        group.MapDelete("/clearimages", async ([FromServices] AppDbContext db) =>
        {
            var allImages = await db.CatImages.ToListAsync();
            db.CatImages.RemoveRange(allImages);
            await db.SaveChangesAsync();
            return Results.Ok("All cat images cleared from memory.");
        }).WithName("ClearCatImages");
    }


}

