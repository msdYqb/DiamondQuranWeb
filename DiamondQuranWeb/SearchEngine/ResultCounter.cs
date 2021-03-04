using DiamondQuranWeb.SearchEngine.Enums;
using DiamondQuranWeb.SearchEngine.Models;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DiamondQuranWeb.Helpers;

namespace DiamondQuranWeb.SearchEngine
{
    internal class ResultCounter
    {
        #region Properties
        public QuranTextType TextSearchType { get; private set; }
        public string Keyword { get; private set; }
        public SearchOptions SelectedSearchOption { get; private set; }
        public int CountMatch { get; private set; }
        public int SurahCount { get; private set; }
        #endregion

        public ResultCounter(QuranTextType quranTextSearchType)
        {
            TextSearchType = quranTextSearchType;
        }

        #region Counter Methods
        internal (int CountMatch, int SurahCount) StartCount(string keyword , SearchOptions searchOption, List<Quran> SearchResult)
        {
            Keyword = keyword;
            SelectedSearchOption = searchOption;

            CountMatch = CountMatches(SearchResult, SelectedSearchOption, Keyword);
            SurahCount = SearchResult.Select(x => x.SurahName).Distinct().Count();
            return (CountMatch, SurahCount);
        }
        #endregion

        #region Counter Queries
        internal int CountMatches(List<Quran> searchResult, SearchOptions searchOption, string keyword)
        {
            var htmlDoc = new HtmlDocument();
            int countMatch = 0;
            foreach (var ayah in searchResult)
            {
                if (searchOption == SearchOptions.Contains) //TODO add counter for all searchres
                    countMatch += Helpers.AllIndexesOf(ayah.GetProperty(TextSearchType), keyword).Count;
                else if (searchOption == SearchOptions.FirstInAyahContains || searchOption == SearchOptions.LastInAyahContains || searchOption == SearchOptions.FirstInSurahContains || searchOption == SearchOptions.LastInSurahContains)
                    countMatch++;
                else if (searchOption == SearchOptions.Matches)
                    countMatch += Regex.Matches(ayah.GetProperty(TextSearchType), "(^| )" + keyword + "($| )").Count;
                else
                {
                    htmlDoc.LoadHtml(ayah.AyahLabel.ToHtmlString());
                    countMatch += htmlDoc.DocumentNode.SelectNodes("//p/span").Count;
                }
            }
            return countMatch;
        }
        internal int CountResult(List<QuranWords> answers, List<Quran> searchResult)
        {
            int countResult = new int();
            foreach (var result in searchResult)
            {
                foreach (var answer in answers)
                {
                    var count = result.AyahText.Split(' ').Where(s => s == answer.Word).Count();
                    if (count > 0)
                        countResult += count;
                }
            }
            return countResult;
        }
        #endregion
    }
}
