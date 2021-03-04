using DiamondQuranWeb.Models;
using DiamondQuranWeb.SearchEngine.Enums;
using DiamondQuranWeb.SearchEngine.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DiamondQuranWeb.Helpers;

namespace DiamondQuranWeb.SearchEngine
{
    internal class WordsSearch : ISearch
    {
        #region Properties
        public string Keyword { get; private set; }
        public SearchOptions SelectedSearchOption { get; private set; }
        public int CountMatch { get; private set; }
        public int SurahCount { get; private set; }
        public List<QuranWords> SearchResult { get; private set; }
        public List<Quran> WordsListSearchResult { get; private set; }
        public bool AddWordsGrammersDuplicate { get; set; }
        public WordsSearchSetting SearchSetting { get; private set; }
        public QuranTextType TextSearchType { get; private set; }
        #endregion

        #region Fields
        private readonly ResultCounter ResultCounter;
        private readonly ResultPainter ResultPainter;
        private readonly VersesSearch VersesSearch;
        readonly ApplicationDbContext data = new ApplicationDbContext();
        #endregion

        public WordsSearch(QuranTextType quranTextSearchType)
        {
            this.TextSearchType = quranTextSearchType;
            VersesSearch = new VersesSearch(TextSearchType);
            ResultPainter = new ResultPainter(TextSearchType);
            ResultCounter = new ResultCounter(TextSearchType);
        }

        #region Search Methods
        public List<T> Search<T>(object searchSetting)
        {
            SearchSetting = searchSetting as WordsSearchSetting;

            if (SearchSetting.WordsListSearch)
            {
                return WordsListSearch(SearchSetting.SelectedWords) as List<T>;
            }

            this.Keyword = SearchSetting.Keyword;
            this.SelectedSearchOption = SearchSetting.SelectedSearchOption;

            List<QuranWords> wordsResult = new List<QuranWords>();

            var searchResult = VersesSearch.Search(Keyword, SelectedSearchOption, true);
            var paintedWords = ResultPainter.GetPaintedWords(searchResult);

            ResultPainter.Keyword = this.Keyword;
            var paintPlaces = ResultPainter.PaintPlacesExtractor(paintedWords);

            WordsGrammersExtractor(paintedWords, ref wordsResult, paintPlaces);

            CountMatch = wordsResult.Count;
            SurahCount = wordsResult.Select(x => new { x.SurahNumber }).Distinct().Count();
            //WordsResultOrder(ref wordsResult);

            return wordsResult as List<T>;
        }
        public async Task<List<T>> SearchAsync<T>(object searchSetting)
        {
            SearchSetting = searchSetting as WordsSearchSetting;

            if (SearchSetting.WordsListSearch)
            {
                //return await WordsListSearch(SearchSetting.SelectedWords) as List<T>;
            }
            return null;
        }
        public List<Quran> WordsListSearch(List<QuranWords> answers)
        {
            List<Quran> duplicatesList = new List<Quran>();

            var searchResult = new List<Quran>();
            foreach (var answer in answers)
            {
                var SrchResult = VersesSearch.Search(answer.Word, SearchOptions.MatchesNoncontiguous, false, true, false).ToList();
                foreach (var rslt in SrchResult)
                {
                    var aya = searchResult.Where(x=> x.ID == rslt.ID).FirstOrDefault();
                    if (aya == null)
                    {
                        ResultPainter.Keyword = answer.Word;
                        var ayahColor = ResultPainter.StartPainting(rslt.GetProperty(TextSearchType), SearchOptions.MatchesNoncontiguous, rslt.ID);
                        searchResult.Add(new Quran() { ID = rslt.ID, AyahText = rslt.GetProperty(TextSearchType), AyahLabel = ayahColor, SurahName = rslt.SurahName, AyahNumber = rslt.AyahNumber, SurahNumber = rslt.SurahNumber, HasComment = rslt.HasComment, InFavorite = rslt.InFavorite });
                    }
                    else
                    {
                        var ayaElement = duplicatesList.Where(x => x.ID == rslt.ID).FirstOrDefault();
                        if (ayaElement == null)
                            duplicatesList.Add(new Quran { ID = rslt.ID });
                    }
                }
            }
            ResultPainter.PaintDuplicates(duplicatesList, answers, ref searchResult);

            CountMatch = ResultCounter.CountResult(answers, searchResult);

            SortSearchResult(ref searchResult);
            SurahCount = searchResult.Select(x => x.SurahName).Distinct().Count();

            WordsListSearchResult = searchResult;

            answers.Clear();
            return WordsListSearchResult;
        }
        #endregion

        #region Essentials
        private void WordsGrammersExtractor(List<string> words, ref List<QuranWords> wordsResult, List<PaintPlace> paintPlaces)
        {
            int id = 0;
            foreach (var word in words)
            {
                wordsResult.Add(new QuranWords { ID = id++, Word = word });
                foreach (var paintPlace in paintPlaces.Where(x => x.Word == word))
                {
                    string[] wordsGrammer;
                    if (TextSearchType.ToString().Contains("Normal"))
                        wordsGrammer = data.Quran.Where(x=> x.AyahNumber == paintPlace.AyahNumber && x.SurahNumber == paintPlace.SurahNumber).Select(x=> x.NormalEnhanced).First().Split(' ');
                    else wordsGrammer = data.Quran.Where(x => x.AyahNumber == paintPlace.AyahNumber && x.SurahNumber == paintPlace.SurahNumber).Select(x => x.UthmaniFull).First().Split(' ');
                    var exist = wordsResult.First(x => x.Word == word)
                                    .WordsWithGrammersList.FirstOrDefault(x => x.Word == wordsGrammer[paintPlace.WordPosition]) == null ? false : true;
                    if (!exist) wordsResult.First(x => x.Word == word).WordsWithGrammersList.Add(new WordsWithGrammers { Word = wordsGrammer[paintPlace.WordPosition] });
                }
            }
            VersesSearch.SetQuranTextSearchType(this.TextSearchType);
        }
        private void WordsResultOrder(ref List<QuranWords> wordsResult)
        {
            foreach (var word in wordsResult)
            {
                var wordSearchResult = VersesSearch.Search(word.Word, SearchOptions.MatchesNoncontiguous, true);
                var order = (from q in wordSearchResult orderby q.AyahNumber orderby q.SurahNumber select q).First();
                word.SurahNumber = order.SurahNumber;
                word.AyahNumber = order.AyahNumber;
            }
            wordsResult = (from q in wordsResult orderby q.AyahNumber orderby q.SurahNumber select q).ToList();
        }
        private void SortSearchResult(ref List<Quran> searchResult)
        {
            searchResult = new List<Quran>(searchResult.OrderBy(i => i.SurahNumber));
            var orderList = new List<Quran>();
            var newList = new List<Quran>();
            var current = searchResult.FirstOrDefault().SurahNumber;
            for (int i = 0; i < searchResult.Count; i++)
            {
                if (searchResult[i].SurahNumber != current)
                {
                    orderList = new List<Quran>(orderList.OrderBy(a => a.AyahNumber));
                    foreach (var element in orderList)
                        newList.Add(new Quran { ID = element.ID, AyahText = element.AyahText, AyahLabel = element.AyahLabel, SurahName = element.SurahName, AyahNumber = element.AyahNumber, SurahNumber = element.SurahNumber, HasComment = element.HasComment, InFavorite = element.InFavorite });
                    orderList.Clear();
                }
                current = searchResult[i].SurahNumber;
                orderList.Add(new Quran { ID = searchResult[i].ID, AyahText = searchResult[i].AyahText, AyahLabel = searchResult[i].AyahLabel, SurahName = searchResult[i].SurahName, AyahNumber = searchResult[i].AyahNumber, SurahNumber = searchResult[i].SurahNumber, HasComment = searchResult[i].HasComment, InFavorite = searchResult[i].InFavorite });
                if (i + 1 == searchResult.Count)
                {
                    foreach (var element in orderList)
                        newList.Add(new Quran { ID = element.ID, AyahText = element.AyahText, AyahLabel = element.AyahLabel, SurahName = element.SurahName, AyahNumber = element.AyahNumber, SurahNumber = element.SurahNumber, HasComment = element.HasComment, InFavorite = element.InFavorite });
                }
            }
            searchResult = newList;
        }
        #endregion
    }
}