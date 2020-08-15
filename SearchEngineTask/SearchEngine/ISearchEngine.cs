using SearchEngineTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngineTask.SearchEngine
{
    public interface ISearchEngine
    {
        ResponseModel GetResposeResult(string searchText);
    }
}
