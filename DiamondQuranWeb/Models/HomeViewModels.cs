using DiamondQuranWeb.SearchEngine;
using DiamondQuranWeb.SearchEngine.Enums;
using DiamondQuranWeb.SearchEngine.Models;
using System.Collections.Generic;
using System.Web.Mvc;
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
    public class QuranViewModel
    {
        public short SurahNumber { get; set; }
        public short AyahNumber { get; set; }
        public short Page { get; set; }
        public List<SelectListItem> SurahList { get; set; }
        public List<SelectListItem> AyahList { get; set; }
    }
    public class LogicsSearchViewModel
    {
        public string SearchOptions { get; set; }
        public QuranTextType QuranTextType { get; set; }
        public SearchOptions SearchOption { get; set; }
    }
}