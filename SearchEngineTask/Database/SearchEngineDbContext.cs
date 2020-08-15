using SearchEngineTask.Database.DbModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngineTask.Database
{
    public class SearchEngineDbContext : DbContext
    {
        
        public DbSet<Search> Searches { get; set; }
        public DbSet<SearchResult> SearchesResults { get; set; }

    }
}
