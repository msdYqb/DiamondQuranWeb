using DiamondQuranWeb.Models;
using DiamondQuranWeb.SearchEngine.Enums;
using DiamondQuranWeb.SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using System.Web;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using DiamondQuranWeb.Helpers;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;

namespace DiamondQuranWeb.SearchEngine
{

    internal partial class VersesSearch : ISearch
    {
        #region Properties
        public string Keyword { get; private set; }
        public SearchOptions SelectedSearchOption { get; private set; }
        public int CountMatch { get; private set; }
        public int SurahCount { get; private set; }
        public List<Quran> SearchResult { get; private set; }
        public List<object> SearchResultObj { get; private set; } = new List<object>();
        public SearchSetting SearchSetting { get; set; }
        public QuranTextType TextSearchType { get; private set; }
        public QuranTextType QuranTextResultType { get; private set; }
        public bool PaintPlacesExtract { get; set; }
        public List<PaintPlace> PaintPositions { get; private set; }
        #endregion

        #region Fields
        private readonly ResultCounter ResultCounter;
        private readonly ResultPainter ResultPainter;
        string TextSearchTypeStr { get => TextSearchType.ToString(); }
        IPrincipal User = HttpContext.Current.User;
        ApplicationDbContext data = new ApplicationDbContext();
        MethodInfo ReflectionSearch
        {
            get => typeof(VersesSearch).GetMethod(SelectedSearchOption.ToString(), BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
        }
        bool IsNoncontiguousSearch
        {
            get
            {
                switch (SelectedSearchOption)
                {
                    case SearchOptions.ContainsNoncontiguous:
                    case SearchOptions.ContainsLettersUnArranged:
                    case SearchOptions.ContainsLettersArranged:
                    case SearchOptions.MatchesNoncontiguous:
                    case SearchOptions.FirstInWord:
                    case SearchOptions.LastInWord:
                        return true;
                    default:
                        return false;
                }
            }
        }
        #endregion

        public VersesSearch(QuranTextType quranTextSearchType)
        {
            this.TextSearchType = quranTextSearchType;
            ResultPainter = new ResultPainter(TextSearchType);
            ResultCounter = new ResultCounter(TextSearchType);
            SetQuranTextSearchType(quranTextSearchType);
        }

        #region Search Methods
        public List<T> Search<T>(object searchSetting)
        {
            SearchSetting = searchSetting as SearchSetting;
            this.Keyword = SearchSetting.Keyword;
            this.SelectedSearchOption = SearchSetting.SelectedSearchOption;
            this.QuranTextResultType = SearchSetting.QuranTextResultType;

            if (IsNoncontiguousSearch)
                SearchResult = SameWordsSearch();
            else SearchResult = SearchInvoke(SelectedSearchOption.ToString(), Keyword);

            StartPaint();
            StartCheckFavorite(SearchResult);
            StartCheckComments(SearchResult);

            if (TextSearchType != QuranTextResultType)
                SetQuranTextResultType(QuranTextResultType);
            (CountMatch, SurahCount) = ResultCounter.StartCount(Keyword, SelectedSearchOption, SearchResult);

            return SearchResult as List<T>;
        }
        public List<Quran> Search(string keyword, SearchOptions searchOption, bool paintAyah)
        {
            Keyword = keyword;
            SelectedSearchOption = searchOption;

            SearchResult = ReflectionSearch.Invoke(this, new object[] { Keyword }) as List<Quran>;
            if (paintAyah)
            {
                StartPaint();
                (CountMatch, SurahCount) = ResultCounter.StartCount(Keyword, SelectedSearchOption, SearchResult);
            }

            return SearchResult;
        }
        public List<Quran> Search(string keyword, SearchOptions searchOption, bool paintAyah, bool addBookmarks, bool count)
        {
            Keyword = keyword;
            SelectedSearchOption = searchOption;

            SearchResult = ReflectionSearch.Invoke(this, new object[] { Keyword }) as List<Quran>;
            if (paintAyah)
            {
                StartPaint();
                if (count)
                    (CountMatch, SurahCount) = ResultCounter.StartCount(Keyword, SelectedSearchOption, SearchResult);
            }
            if (addBookmarks)
            {
                StartCheckComments(SearchResult);
                StartCheckFavorite(SearchResult);
            }

            return SearchResult;
        }
        List<Quran> SameWordsSearch()
        {
            ResultPainter.PaintPlacesExtract = true;
            List<Quran> searchresult = new List<Quran>();
            var paintPlaces = new List<PaintPlace>();
            var keyword = from s in Keyword.Split(' ')
                          orderby s.Length ascending
                          select s;
            int counter = 0;
            foreach (var item in keyword)
            {
                SearchResultObj = new List<object>();
                if (counter > 0)
                    Database = searchresult.AsQueryable();
                searchresult = SearchInvoke(SelectedSearchOption.ToString(), item);
                SortSearchResultByLength(ref searchresult);
                ResultPainter.Keyword = item;
                searchresult = ResultPainter.StartPainting(searchresult, SelectedSearchOption);
                paintPlaces = ResultPainter.PaintPositions;
                ResultPainter.RemoveFirstPaintedWords(searchresult, ref paintPlaces);
                counter++;
            }

            void SortSearchResultByLength(ref List<Quran> sr)
            {
                foreach (var ayah in sr)
                {
                    var verse = SortByLength(ayah.GetProperty(TextSearchType));
                    ayah.SetProperty(TextSearchTypeStr, verse);
                }
            }
            string SortByLength(string words)
            {
                var wordsStr = "";
                var wordsArray = from s in words.Split(' ') orderby s.Length ascending select s;
                foreach (var item in wordsArray)
                {
                    wordsStr += item + " ";
                }
                return wordsStr.Trim();
            }
            SetQuranTextSearchType(TextSearchType);
            foreach (var verse in searchresult)
            {
                var id = verse.ID;
                var fullVerse = Database.Single($"ID == \"{verse.ID}\"");
                Quran quran = JsonConvert.DeserializeObject<Quran>(JsonConvert.SerializeObject(fullVerse));
                verse.SetProperty(TextSearchType.ToString(), quran.GetProperty(TextSearchType));
            }
            return searchresult;
        }
        public Task<List<T>> SearchAsync<T>(object searchSetting)
        {
            throw new NotImplementedException();
        }
        List<Quran> SearchInvoke(string searchOption, string keyword)
        {
            Dictionary<string, Func<string, List<Quran>>> proc = new Dictionary<string, Func<string, List<Quran>>>
{
   {"Contains", Contains},
   {"ContainsNoncontiguous", ContainsNoncontiguous},
   {"ContainsLettersUnArranged", ContainsLettersUnArranged},
   {"ContainsLettersArranged", ContainsLettersArranged},
   {"Matches", Matches},
   {"MatchesNoncontiguous", MatchesNoncontiguous},
   {"FirstInAyahContains", FirstInAyahContains},
   {"LastInAyahContains", LastInAyahContains},
   {"FirstInSurahContains", FirstInSurahContains},
   {"LastInSurahContains", LastInSurahContains},
   {"FirstInWord", FirstInWord},
   {"LastInWord", LastInWord},
};
            return proc[searchOption].Invoke(keyword);
        }
        #endregion

        #region Essentials
        private void StartPaint()
        {
            if (TextSearchType != QuranTextResultType)
                PaintPlacesExtract = true;

            ResultPainter.Keyword = this.Keyword;
            ResultPainter.PaintPlacesExtract = this.PaintPlacesExtract;

            SearchResult = ResultPainter.StartPainting(SearchResult, SelectedSearchOption);
            if (PaintPlacesExtract)
                PaintPositions = ResultPainter.PaintPositions;
        }
        [Authorize]
        public List<Quran> StartCheckComments(List<Quran> searchResult)
        {
            var userId = User.Identity.GetUserId();
            foreach (var comment in data.Comments.Where(x => x.User.Id == userId).ToList())
            {
                var verse = searchResult.SingleOrDefault(x => x.ID == comment.Quran.ID);
                if (verse != null)
                    verse.HasComment = true;
            }
            return searchResult;
        }
        [Authorize]
        public List<Quran> StartCheckFavorite(List<Quran> searchResult)
        {
            var userId = User.Identity.GetUserId();
            using (var data = new ApplicationDbContext())
            {
                foreach (var favoriteList in data.FavoritesList.Where(x => x.User.Id == userId).ToList())
                {
                    foreach (var favorites in data.Favorites.Where(x => x.FavoriteList.ID == favoriteList.ID).ToList())
                    {
                        foreach (var verse in searchResult.Where(x => x.ID == favorites.Quran.ID))
                            verse.InFavorite = true;
                    }
                }
            }
            return searchResult;
        }
        public void SetQuranTextSearchType(QuranTextType quranTextSearchType)
        {
            Database = data.Quran.Select($"new {{ ID, {quranTextSearchType.ToString()} , SurahName , SurahNumber , AyahNumber }}");
        }
        public void SetQuranTextResultType(QuranTextType quranTextResultType)
        {
            var searchResult = SearchResult;
            ResultPainter.PaintsExtender(ref searchResult, PaintPositions, quranTextResultType, false);
        }
        #endregion

        #region Search Queries
        internal List<Quran> Contains(string keyword)
        {
            return ConvertToList(Database.Where(TextSearchTypeStr + ".Contains(@0)", new string[] { keyword }));
        }

        internal List<Quran> ContainsNoncontiguous(string keyword)
        {
            string[] keywords = keyword.Split(' ');
            foreach (var ayah in Database)
            {
                var found = 0;
                foreach (var k in keywords)
                {
                    if (ayah.GetProperty(TextSearchType).Contains(k))
                        found++;
                }
                if (found == keywords.Count())
                    SearchResultObj.Add(ayah);
            }
            return ConvertToList(SearchResultObj);
        }

        internal List<Quran> ContainsLettersUnArranged(string keyword)
        {
            string[] keywords = keyword.Split(' ');
            foreach (var ayah in Database)
            {
                var found = 0;
                foreach (var k in keywords)
                {
                    if (ayah.GetProperty(TextSearchType).Split(' ').Any(w => !k.Except(w).Any())) found++;
                }
                if (found == keywords.Count())
                    SearchResultObj.Add(ayah);
            }
            return ConvertToList(SearchResultObj);
        }
        internal List<Quran> ContainsLettersArranged(string keyword)
        {
            string[] keywords = keyword.Split(' ');
            foreach (var ayah in Database)
            {
                var keywordsList = keywords.ToList();
                var ayahWords = ayah.GetProperty(TextSearchType).Split(' ');
                foreach (var word in ayahWords)
                {
                    bool found = false;
                    var keywordsFound = new List<string>();
                    var wordLetters = word.ToCharArray();
                    foreach (var k in keywordsList)
                    {
                        int position = 0;
                        foreach (var letter in wordLetters)
                        {
                            if (position + 1 <= k.Length)
                                if (letter == k[position])
                                {
                                    if (position + 1 == k.Length) { keywordsFound.Add(k); found = true; break; }
                                    position++;
                                }
                        }
                    }
                    if (found)
                    {
                        foreach (var kfound in keywordsFound)
                            keywordsList.Remove(kfound);
                        if (keywordsList.Count == 0)
                        {
                            SearchResultObj.Add(ayah);
                            break;
                        }
                    }
                }
            }

            return ConvertToList(SearchResultObj);
        }
        internal List<Quran> Matches(string keyword)
        {
            var rx = new Regex("(^| )" + keyword + "($| )", RegexOptions.IgnoreCase);
            foreach (var ayah in Database)
            {
                var verse = ayah.GetProperty(TextSearchType);
                if (rx.IsMatch(verse)) SearchResultObj.Add(ayah);
            }
            return ConvertToList(SearchResultObj);
        }
        internal List<Quran> MatchesNoncontiguous(string keyword)
        {
            string[] keywords = keyword.Split(' ');

            foreach (var ayah in Database)
            {
                var ayahWords = ayah.GetProperty(TextSearchType).Split(' ');
                var found = 0;
                foreach (var k in keywords)
                {
                    var word = ayahWords.Where(s => s == k).FirstOrDefault();
                    if (word != null)
                        found++;
                }
                if (found == keywords.Count())
                    SearchResultObj.Add(ayah);
            }

            return ConvertToList(SearchResultObj);
        }
        internal List<Quran> FirstInAyahContains(string keyword)
        {
            string[] keywords = keyword.Split(' ');

            foreach (var ayah in Database)
            {
                var ayahWords = ayah.GetProperty(TextSearchType).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var firstWords = string.Empty;
                for (int i = 0; i < keywords.Count(); i++)
                {
                    if (i <= ayahWords.Length - 1)
                        firstWords += ayahWords[i] + " ";
                }
                firstWords = firstWords.Trim();
                if (firstWords.Contains(keyword))
                    SearchResultObj.Add(ayah);
            }

            return ConvertToList(SearchResultObj);
        }
        internal List<Quran> LastInAyahContains(string keyword)
        {
            string[] keywords = keyword.Split(' ');

            foreach (var ayah in Database)
            {
                var ayahWords = ayah.GetProperty(TextSearchType).Split(' ');
                Array.Reverse(ayahWords);
                var lastWords = new List<string>();
                for (int i = 0; i < keywords.Count(); i++)
                {
                    if (i <= ayahWords.Count() - 1)
                        lastWords.Add(ayahWords[i]);
                }
                lastWords.Reverse();
                var lastWordsStr = "";
                foreach (var word in lastWords)
                {
                    lastWordsStr += word + " ";
                }
                lastWordsStr = lastWordsStr.Trim();
                if (lastWordsStr.Contains(keyword))
                    SearchResultObj.Add(ayah);
            }

            return ConvertToList(SearchResultObj);
        }
        internal List<Quran> FirstInSurahContains(string keyword)
        {
            List<Quran> searchResult = new List<Quran>();
            string[] keywords = keyword.Split(' ');

            var allFirstAyah = Database.Where("AyahNumber == \"1\"");
            foreach (var ayah in allFirstAyah)
            {
                var ayahWords = ayah.GetProperty(TextSearchType).Split(' ');
                var firstWords = string.Empty;
                for (int i = 0; i < keywords.Count(); i++)
                {
                    if (i <= ayahWords.Length - 1)
                        firstWords += ayahWords[i] + " ";
                }
                firstWords = firstWords.Trim();
                if (firstWords.Contains(keyword))
                    SearchResultObj.Add(ayah);
            }
            return ConvertToList(SearchResultObj);
        }

        internal List<Quran> LastInSurahContains(string keyword)
        {
            string[] keywords = keyword.Split(' ');

            List<object> allLastAyah = new List<object>();
            for (int i = 0; i < 114; i++)
            {
                var surah = i;
                ++surah;
                var lastAyah = Database.Where($"SurahNumber == \"{surah}\"").ToDynamicList().OrderByDescending(o => o.ID).FirstOrDefault();
                allLastAyah.Add(lastAyah);
            }

            foreach (var ayah in ConvertToList(allLastAyah))
            {
                var ayahWords = ayah.GetProperty(TextSearchType).Split(' ');
                Array.Reverse(ayahWords);
                var lastWords = new List<string>();
                for (int i = 0; i < keywords.Count(); i++)
                {
                    if (i <= ayahWords.Count() - 1)
                        lastWords.Add(ayahWords[i]);
                }
                lastWords.Reverse();
                var lastWordsStr = "";
                foreach (var word in lastWords)
                {
                    lastWordsStr += word + " ";
                }
                lastWordsStr = lastWordsStr.Trim();
                if (lastWordsStr.Contains(keyword))
                    SearchResultObj.Add(ayah);
            }

            return ConvertToList(SearchResultObj);
        }
        internal List<Quran> FirstInWord(string keyword)
        {
            string[] keywords = keyword.Split(' ');

            foreach (var ayah in Database)
            {
                var ayahWords = ayah.GetProperty(TextSearchType).Split(' ');
                var addVerse = false;
                foreach (var word in ayahWords)
                {
                    var wordC = word.ToCharArray();
                    var keywordC = keywords[0].ToCharArray();
                    int found = 0;
                    for (int i = 0; i < keywords[0].ToCharArray().Count(); i++)
                    {
                        if (i <= wordC.Count() - 1)
                            if (keywordC[i] == wordC[i])
                                found++;
                    }
                    if (found == keywords[0].ToCharArray().Count())
                        addVerse = true;
                }
                if (addVerse) SearchResultObj.Add(ayah);
            }

            return ConvertToList(SearchResultObj);
        }
        internal List<Quran> LastInWord(string keyword)
        {
            string[] keywords = keyword.Split(' ');

            foreach (var ayah in Database)
            {
                var ayahWords = ayah.GetProperty(TextSearchType).Split(' ');
                var addVerse = false;
                foreach (var word in ayahWords)
                {
                    var wordC = word.ToCharArray().Reverse().ToList();
                    var keywordC = keywords[0].ToCharArray().Reverse().ToList();
                    int found = 0;
                    for (int i = 0; i < keywords[0].ToCharArray().Count(); i++)
                    {
                        if (i <= wordC.Count() - 1)
                            if (keywordC[i] == wordC[i])
                                found++;
                    }
                    if (found == keywords[0].ToCharArray().Count())
                        addVerse = true;
                }
                if (addVerse) SearchResultObj.Add(ayah);
            }

            return ConvertToList(SearchResultObj);
        }
        #endregion
    }
}
