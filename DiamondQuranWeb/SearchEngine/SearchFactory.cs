using DiamondQuranWeb.SearchEngine.Enums;
using DiamondQuranWeb.SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiamondQuranWeb.SearchEngine
{
    public class SearchFactory
    {
        public ISearch ISearch { get; set; }
        List<ISearch> ISearchList = new List<ISearch>();
        public SearchFactory(QuranTextType quranTextSearchType)
        {
            ISearchList.Add(new VersesSearch(quranTextSearchType));
            ISearchList.Add(new WordsSearch(quranTextSearchType));
            ISearchList.Add(new LogicsSearch(quranTextSearchType));
        }
    
        private ISearch Set(Type searchType)
        {
            return ISearchList.First(x => x.GetType() == searchType);
        }
        public List<Quran> VersesSearch(SearchSetting searchSetting)
        {
            ISearch = Set(typeof(VersesSearch));
            return ISearch.Search<Quran>(searchSetting);
        }
        public List<QuranWords> WordsSearch(WordsSearchSetting searchSetting)
        {
            ISearch = Set(typeof(WordsSearch));
            return ISearch.Search<QuranWords>(searchSetting);
        }
        public List<Quran> LogicsSearch(LogicsSearchSetting searchSetting)
        {
            ISearch = Set(typeof(LogicsSearch));
            return ISearch.Search<Quran>(searchSetting);
        }
        public List<Quran> WordsListSearch(WordsSearchSetting searchSetting)
        {
            ISearch = Set(typeof(WordsSearch));
            return ISearch.Search<Quran>(searchSetting);
        }
        public Task<List<Quran>> WordsListSearchAsync(WordsSearchSetting searchSetting)
        {
            ISearch = Set(typeof(WordsSearch));
            return ISearch.SearchAsync<Quran>(searchSetting);
        }
    }
}