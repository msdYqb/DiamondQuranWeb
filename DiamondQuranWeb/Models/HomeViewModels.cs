using DiamondQuranWeb.SearchEngine;
using DiamondQuranWeb.SearchEngine.Enums;
using DiamondQuranWeb.SearchEngine.Models;
using Tafsirs = DiamondQuranWeb.SearchEngine.Enums.TafsirsNames;

namespace DiamondQuranWeb.Models
{
    public class BookmarksViewModel
    {
        public List<FavoritesList> FavoritesList { get; set; }
        public List<Comments> Comments { get; set; }
        public string FavoriteList { get; set; }
    }
    public class SearchPostViewModel
    {
        public string Keyword { get; set; }
        public SearchOptions SearchOption { get; set; }
        public QuranTextType QuranTextSearchType { get; set; }
        public QuranTextType QuranTextResultType { get; set; }
        public bool SearchByWords { get; set; }
        public bool LogicsSearch { get; set; }
    }
    public class SearchResultViewModel
    {
        public ISearch ISearch { get; set; }
        public List<Quran> SearchResultList { get; set; }
        public SearchPostViewModel SearchPost { get; set; }
        public Tafsirs Tafsirs { get; set; }
        public string CommentText { get; set; }
    }
    public class WordsSearchResultViewModel
    {
        public ISearch ISearch { get; set; }
        public List<QuranWords> WordsSearchResultList { get; set; }
    }
    public class LogicsSearchViewModel
    {
        public string SearchOptions { get; set; }
        public QuranTextType QuranTextType { get; set; }
        public SearchOptions SearchOption { get; set; }
    }
    public class FavoritesViewModel
    {
        public List<Favorites> Favorites { get; set; }
        public string ListName { get; set; }
        public Tafsirs Tafsirs { get; set; }
    }
    public class QuranViewModel
    {
        public List<QuranIndex> QuranIndex { get; set; }
        public string CurrentDomain { get; set; }
        public int SurahNumber { get; set; }
        public int AyahNumber { get; set; }
        public bool NavBySearch { get; set; }
    }
}