using DiamondQuranWeb.Models;
using DiamondQuranWeb.SearchEngine;
using DiamondQuranWeb.SearchEngine.Enums;
using DiamondQuranWeb.SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using DiamondQuranWeb.Helpers;
using Tafsirs = DiamondQuranWeb.SearchEngine.Enums.TafsirsNames;
using System.Diagnostics;
using System.Linq.Dynamic.Core;

namespace DiamondQuranWeb.Controllers
{
    public partial class HomeController : BaseController
    {
        public ActionResult View1()
        {
            return View();
        }

        [ActionName("Main")]
        [HttpGet]
        public ActionResult Main()
        {
            return View();
        }

        [ActionName("Guide")]
        [HttpGet]
        public ActionResult Guide()
        {
            return View();
        }

        [ActionName("Search")]
        [HttpGet]
        public ActionResult Search_Get(SearchPostViewModel searchModel)
        {
            SearchResultViewModel searchResult = new SearchResultViewModel();
            var searchFactory = new SearchFactory(searchModel.QuranTextSearchType);
            if (searchModel.SearchByWords)
            {
                var selectedWordsList = (List<QuranWords>)Session["SelectedWordsListSession"];
                searchResult.SearchResultList = searchFactory.WordsListSearch(new WordsSearchSetting { SelectedWords = selectedWordsList, WordsListSearch = true });
            }
            else if (searchModel.LogicsSearch)
            {
                var logicsSearchSetting = (LogicsSearchSetting)TempData["logicsSearchSetting"];
                searchResult.SearchResultList = searchFactory.LogicsSearch(logicsSearchSetting);
            }
            else
            {
                searchResult.SearchResultList = searchFactory.VersesSearch(new SearchSetting { Keyword = searchModel.Keyword, SelectedSearchOption = searchModel.SearchOption, QuranTextResultType = searchModel.QuranTextResultType }); //TODO ى ابدالها ب ي 
            }
            searchResult.ISearch = searchFactory.ISearch;
            searchResult.SearchPost = searchModel;
            return View(searchResult);
        }
        [ActionName("Search")]
        [HttpPost]
        public ActionResult Search_Post(SearchResultViewModel searchModel)
        {
            if (Session["SearchByWords"] is bool searchByWords && searchByWords)
            {
                return RedirectToAction("WordsSearchResult", searchModel.SearchPost);
            }
            return RedirectToAction("Search", "Home", searchModel.SearchPost);
        }

        [ActionName("WordsSearchResult")]
        [HttpGet]
        public ActionResult WordsSearchResult(SearchPostViewModel searchModel)
        {
            WordsSearchResultViewModel wordsSearchResult = new WordsSearchResultViewModel();
            var searchFactory = new SearchFactory(searchModel.QuranTextSearchType);
            wordsSearchResult.WordsSearchResultList = searchFactory.WordsSearch(new WordsSearchSetting { Keyword = searchModel.Keyword, SelectedSearchOption = searchModel.SearchOption });
            wordsSearchResult.ISearch = searchFactory.ISearch;
            return View(wordsSearchResult);
        }

        [HttpPost]
        public ActionResult SearchBySelectedWords(List<QuranWords> selectedWords, QuranTextType quranTextSearchType)
        {
            Session["SelectedWordsListSession"] = selectedWords;
            return Json(new { redirectToUrl = Url.Action("Search", "Home", new { SearchByWords = true, quranTextSearchType }) });
        }

        [HttpPost]
        public ActionResult ChangeSearchResultTextType(List<int> versesIds, QuranTextType quranTextType)
        {
            var verses = new List<Quran>();

            foreach (var id in versesIds)
            {
                var verse = data.Quran.Single(x => x.ID == id);
                var verseText = verse.GetPropValue(quranTextType.ToString()) as string;
                verses.Add(new Quran { ID = verse.ID, AyahText = verseText });
            }

            return Json(new { verses });
        }

        [HttpPost]
        public ActionResult ChangeTafsir(int verseId, Tafsirs tafsir)
        {
            var tafsirText = (data.Tafsirs.Single(x => x.QuranCleaner.ID == verseId).GetPropValue(tafsir.ToString()) as string);

            return Json(new { tafsirText });
        }

        [HttpGet]
        public ActionResult LogicsSearch()
        {
            var logicsSearchModel = new LogicsSearchViewModel
            {
                SearchOptions = "[ \n"
            };
            foreach (var searchType in Enum.GetValues(typeof(SearchOptions)))
            {
                SearchOptions searchOption = (SearchOptions)Enum.Parse(typeof(SearchOptions), searchType.ToString());
                logicsSearchModel.SearchOptions += $"'{searchOption.GetDisplayName()}',{Environment.NewLine}";
            }
            logicsSearchModel.SearchOptions += "]";
            ViewBag.Message = "Your application description page.";

            return View(logicsSearchModel);
        }

        public ActionResult LogicsSearchAjaxPost(LogicsSearchSetting logicsSearchSetting)
        {
            TempData["logicsSearchSetting"] = logicsSearchSetting;
            return Json(new { redirectToUrl = Url.Action("Search", "Home", new RouteValueDictionary(new SearchPostViewModel { LogicsSearch = true })) });
        }

        public List<string> GetAyahList(QuranTextType quranTextType)
        {
            return data.Quran
             .Select(quranTextType.ToString())
             .ToDynamicList().Cast<string>().ToList();
        }

        [HttpPost]
        public JsonResult AutoComplete(string keyword,QuranTextType quranTextType)
        {
            if (Session["AyahAutoComplete"] == null)
            {
                Session["AyahAutoComplete"] = GetAyahList(quranTextType);
            }
            var ayahList = Session["AyahAutoComplete"] as List<string>;
            var searchResult = (from t in ayahList where t.Contains(keyword) select t).Take(5);
            return Json(searchResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeAutoComplete(QuranTextType quranTextType)
        {
            Debug.WriteLine("ChangeAutoComplete");
            Session["AyahAutoComplete"] = GetAyahList(quranTextType);
            return new JsonResult();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}