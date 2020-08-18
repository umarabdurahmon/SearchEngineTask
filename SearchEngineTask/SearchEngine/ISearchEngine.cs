using SearchEngineTask.Models;

namespace SearchEngineTask.SearchEngine
{
    public interface ISearchEngine
    {
        ResponseModel GetResposeResult(string searchText);
    }
}