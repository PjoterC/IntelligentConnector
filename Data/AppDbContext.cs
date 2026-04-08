using IntelligentConnector.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace IntelligentConnector.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CatFact> CatFacts { get; set; }
}