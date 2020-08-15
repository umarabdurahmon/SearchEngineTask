using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngineTask.Database.DbModels
{
    public class Search
    {
        public Search()
        {
            SearchResults = new List<SearchResult>();
        }

        [Key]
        public int SearchId { get; set; }
        public string SearchText { get; set; }
        public string SearchEngineName { get; set; }

        public IList<SearchResult> SearchResults { get; set; }
    }
}
