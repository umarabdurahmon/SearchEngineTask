using SearchEngineTask.Database.DbModels;
using System.Data.Entity;

namespace SearchEngineTask.Database
{
    public class SearchEngineDbContext : DbContext
    {
        public DbSet<Search> Searches { get; set; }
        public DbSet<SearchResult> SearchesResults { get; set; }
    }
}