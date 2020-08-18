using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchEngineTask.Database.DbModels
{
    public class SearchResult
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }

        [ForeignKey("Search")]
        public int SearchId { get; set; }

        public Search Search { get; set; }
    }
}