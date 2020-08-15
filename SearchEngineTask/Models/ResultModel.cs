using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngineTask.Models
{
    public class ResultModel
    {
        public ResultModel()
        {
            Title = string.Empty;
            Link = string.Empty;
            Description = string.Empty;
        }

        public ResultModel(string link, string title, string description)
        {
            Title = title == null ? string.Empty : title;
            Link = link == null ? string.Empty : link;
            Description = description == null ? string.Empty : description;
        }


        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
    }
}
