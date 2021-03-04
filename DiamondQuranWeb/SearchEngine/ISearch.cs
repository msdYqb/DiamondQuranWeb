using DiamondQuranWeb.SearchEngine.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondQuranWeb.SearchEngine
{
    public interface ISearch
    {
        string Keyword { get; }
        SearchOptions SelectedSearchOption { get; }
        int CountMatch { get; }
        int SurahCount { get; }
        List<T> Search<T>(object searchSetting);
        Task<List<T>> SearchAsync<T>(object searchSetting);
    }
}