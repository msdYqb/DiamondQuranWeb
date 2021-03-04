using DiamondQuranWeb.Models;
using DiamondQuranWeb.SearchEngine.Enums;
using DiamondQuranWeb.SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiamondQuranWeb.Helpers;
using Newtonsoft.Json;

namespace DiamondQuranWeb.SearchEngine
{
    internal class LogicsSearch : ISearch
    {
        #region Properties
        public string Keyword { get; private set; }
        public SearchOptions SelectedSearchOption { get; private set; }
        public int CountMatch { get; private set; }
        public int SurahCount { get; private set; }
        public List<Quran> SearchResult { get; private set; }
        public LogicsSearchSetting LogicsSearchSetting { get; private set; }
        public QuranTextType TextSearchType { get; private set; }
        public QuranTextType TextResultType { get; private set; }
        #endregion

        #region Fields
        private readonly ResultCounter ResultCounter;
        private VersesSearch VersesSearch;
        private readonly ResultPainter ResultPainter;
        ApplicationDbContext data = new ApplicationDbContext();
        #endregion

        public LogicsSearch(QuranTextType quranTextSearchType)
        {
            TextSearchType = quranTextSearchType;
            VersesSearch = new VersesSearch(quranTextSearchType) { PaintPlacesExtract = true };
            ResultPainter = new ResultPainter(TextSearchType);
            ResultCounter = new ResultCounter(TextSearchType);
        }

        #region Search Queries
        internal (List<Quran> searchResult, int countMatch, int surahCount) LogicSearchAnd()
        {
            var searchResult = new List<Quran>();
            var paintPlaces = new List<PaintPlace>();
            if (LogicsSearchSetting.Within)
            {
                var counter = 0;
                foreach (var item in LogicsSearchSetting.SelectedOptions1)
                {
                    if (counter > 0)
                        VersesSearch.Database = searchResult.AsQueryable();
                    searchResult = VersesSearch.Search(item.Keyword, item.SearchOption, true, true, false).ToList();
                    paintPlaces.AddRange(VersesSearch.PaintPositions);
                    counter++;
                }
                VersesSearch.SetQuranTextSearchType(TextSearchType);
            }
            else
            {
                foreach (var item in LogicsSearchSetting.SelectedOptions1)
                {
                    var search = VersesSearch.Search(item.Keyword, item.SearchOption, true, true, false);
                    paintPlaces.AddRange(VersesSearch.PaintPositions);
                    foreach (var a in search)
                    {
                        if (searchResult.Where(x => x.ID == a.ID).FirstOrDefault() == null)
                        {
                            searchResult.Add(a);
                        }
                    }
                }
            }

            ResultPainter.PaintsExtender(ref searchResult, paintPlaces, TextResultType, false);
            foreach (var item in LogicsSearchSetting.SelectedOptions1)
                CountMatch += ResultCounter.CountMatches(searchResult, item.SearchOption, item.Keyword);
            SurahCount = searchResult.Select(x => x.SurahName).Distinct().Count();
            SearchResultOrder(searchResult, out var searchResultOrder);
            SearchResult = new List<Quran>(searchResultOrder);

            return (SearchResult, CountMatch, SurahCount);
        }
        internal (List<Quran> searchResult, int countMatch, int surahCount) LogicSearchOr()
        {
            var searchResult = new List<Quran>();
            var paintPlaces = new List<PaintPlace>();
            foreach (var item in LogicsSearchSetting.SelectedOptions1)
            {
                var search = VersesSearch.Search(item.Keyword, item.SearchOption, true, true, false);
                paintPlaces.AddRange(VersesSearch.PaintPositions);
                CountMatch += ResultCounter.CountMatches(search, item.SearchOption, item.Keyword);
                foreach (var a in search)
                {
                    if (searchResult.Where(x => x.ID == a.ID).FirstOrDefault() == null)
                    {
                        searchResult.Add(a);
                    }
                }
            }
            ResultPainter.PaintsExtender(ref searchResult, paintPlaces, TextResultType, false);
            SurahCount = searchResult.Select(x => x.SurahName).Distinct().Count();
            SearchResultOrder(searchResult, out var searchResultOrder);
            SearchResult = new List<Quran>(searchResultOrder);
            return (SearchResult, CountMatch, SurahCount);
        }
        internal (List<Quran> searchResult, int countMatch, int surahCount) LogicSearchNot()
        {
            var searchResult = new List<Quran>();
            var notSearchResult = new List<Quran>();
            foreach (var item in LogicsSearchSetting.SelectedOptions1)
            {
                var search = VersesSearch.Search(item.Keyword, item.SearchOption, false);
                searchResult.AddRange(search);
            }
            foreach (var item in VersesSearch.Database)
            {
                var id = (short)item.GetProperty("ID");
                var exist = searchResult.Where(x => x.ID == id).FirstOrDefault() == null ? false : true;
                if (!exist)
                {
                    var ayah = JsonConvert.DeserializeObject<Quran>(JsonConvert.SerializeObject(item));
                    notSearchResult.Add(ayah);
                }
            }

            CheckFavoriteAndComments(notSearchResult);
            ResultPainter.AddPaint(ref notSearchResult);
            SearchResult = notSearchResult;
            foreach (var item in SearchResult)
                CountMatch += item.GetProperty(TextSearchType).Split(' ').Count();
            SurahCount = notSearchResult.Select(x => x.SurahName).Distinct().Count();

            return (SearchResult, CountMatch, SurahCount);
        }
        internal (List<Quran> searchResult, int countMatch, int surahCount) LogicSearchAndNot()
        {
            var searchResult = new List<Quran>();
            var paintPlaces = new List<PaintPlace>();
            if (LogicsSearchSetting.Within)
            {
                var counter = 0;
                foreach (var item in LogicsSearchSetting.SelectedOptions1)
                {
                    if (counter > 0)
                        VersesSearch.Database = searchResult.AsQueryable();
                    searchResult = VersesSearch.Search(item.Keyword, item.SearchOption, true, true, false).ToList();
                    paintPlaces.AddRange(VersesSearch.PaintPositions);
                    counter++;
                }
                VersesSearch.SetQuranTextSearchType(TextSearchType);
            }
            else
            {
                foreach (var item in LogicsSearchSetting.SelectedOptions1)
                {
                    var search = VersesSearch.Search(item.Keyword, item.SearchOption, true, true, false);
                    paintPlaces.AddRange(VersesSearch.PaintPositions);
                    foreach (var a in search)
                    {
                        if (searchResult.Where(x => x.ID == a.ID).FirstOrDefault() == null)
                        {
                            searchResult.Add(a);
                        }
                    }
                }
            }
            foreach (var item in LogicsSearchSetting.SelectedOptions2)
            {
                var notSearch = VersesSearch.Search(item.Keyword, item.SearchOption, true);
                foreach (var ns in notSearch)
                {
                    var row = searchResult.Where(x => x.ID == ns.ID).FirstOrDefault();
                    if (row != null)
                    {
                        searchResult.Remove(row);
                    }
                }
            }

            ResultPainter.PaintsExtender(ref searchResult, paintPlaces, TextResultType, false);
            foreach (var item in LogicsSearchSetting.SelectedOptions1)
                CountMatch += ResultCounter.CountMatches(searchResult, item.SearchOption, item.Keyword);
            SurahCount = searchResult.Select(x => x.SurahName).Distinct().Count();
            SearchResultOrder(searchResult, out var searchResultOrder);
            SearchResult = new List<Quran>(searchResult);

            return (SearchResult, CountMatch, SurahCount);
        }
        #endregion

        #region Search Methods
        public List<T> Search<T>(object searchSetting)
        {
            LogicsSearchSetting = searchSetting as LogicsSearchSetting;

            switch (LogicsSearchSetting.LogicSearchChoice)
            {
                case LogicsSearchChoice.And:
                    (SearchResult, CountMatch, SurahCount) = LogicSearchAnd();
                    break;
                case LogicsSearchChoice.Or:
                    (SearchResult, CountMatch, SurahCount) = LogicSearchOr();
                    break;
                case LogicsSearchChoice.Not:
                    (SearchResult, CountMatch, SurahCount) = LogicSearchNot();
                    break;
                case LogicsSearchChoice.AndNot:
                    (SearchResult, CountMatch, SurahCount) = LogicSearchAndNot();
                    break;
                default:
                    break;
            }

            return SearchResult as List<T>;
        }
        public Task<List<T>> SearchAsync<T>(object searchSetting)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Essentials
        private void SearchResultOrder(List<Quran> searchResult, out IOrderedEnumerable<Quran> searchResultOrder)
        {
            searchResultOrder = from q in searchResult orderby q.AyahNumber orderby q.SurahNumber select q;
        }

        void CheckFavoriteAndComments(List<Quran> searchResult)
        {
            VersesSearch.StartCheckFavorite(searchResult);
            VersesSearch.StartCheckComments(searchResult);
        }
        #endregion

    }
}