using DiamondQuranWeb.Models;
using DiamondQuranWeb.SearchEngine;
using DiamondQuranWeb.SearchEngine.Enums;
using DiamondQuranWeb.SearchEngine.Models;
using DiamondQuranWeb.Helpers;
using Tafsirs = DiamondQuranWeb.SearchEngine.Enums.TafsirsNames;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;

namespace DiamondQuranWeb.Controllers
{
    public partial class HomeController : BaseController
    {
        [HttpGet("")]
        [HttpGet("Main")]
        public ActionResult Main()
        {
            if (!SessionExist("AyahAutoComplete"))
            {
                var quranList = DbContext.Quran.Select(x => x.NormalCleanest).ToList();
                SetSession("AyahAutoComplete", quranList);
            }
            return View();
        }

        [HttpGet("Guide")]
        public ActionResult Guide()
        {
            return View();
        }

        [HttpGet("Search")]
        public ActionResult Search(SearchPostViewModel searchModel)
        {
            SearchResultViewModel searchResult = new();
            var searchFactory = new SearchFactory(searchModel.QuranTextSearchType, DbContext.Quran);
            if (searchModel.SearchByWords)
            {
                var selectedWordsList = GetSession<List<QuranWords>>("SelectedWordsListSession");
                searchResult.SearchResultList = searchFactory.WordsListSearch(new WordsSearchSetting { SelectedWords = selectedWordsList, WordsListSearch = true });
            }
            else if (searchModel.LogicsSearch)
            {
                var logicsSearchSetting = TempData.Get<LogicsSearchSetting>("logicsSearchSetting");
                searchResult.SearchPost = new SearchPostViewModel
                {
                    Keyword = GetKeywords(logicsSearchSetting)
                };
                searchResult.SearchResultList = searchFactory.LogicsSearch(logicsSearchSetting);
            }
            else
            {
                searchResult.SearchResultList = searchFactory.VersesSearch(new SearchSetting { Keyword = searchModel.Keyword, SelectedSearchOption = searchModel.SearchOption, QuranTextResultType = searchModel.QuranTextResultType }); //TODO ى ابدالها ب ي 
            }
            searchResult.ISearch = searchFactory.ISearch;
            if (!searchModel.LogicsSearch)
                searchResult.SearchPost = searchModel;
            return View(searchResult);
        }
        [HttpPost("Search")]
        public ActionResult Search(SearchResultViewModel searchModel)
        {
            var searchByWords = GetSession<bool>("SearchByWords");
            if (searchByWords)
            {
                return RedirectToAction("WordsSearchResult", searchModel.SearchPost);
            }
            return RedirectToAction("Search", "Home", searchModel.SearchPost);
        }

        [HttpGet("WordsSearchResult")]
        public ActionResult WordsSearchResult(SearchPostViewModel searchModel)
        {
            WordsSearchResultViewModel wordsSearchResult = new WordsSearchResultViewModel();
            var searchFactory = new SearchFactory(searchModel.QuranTextSearchType,this.DbContext.Quran);
            wordsSearchResult.WordsSearchResultList = searchFactory.WordsSearch(new WordsSearchSetting { Keyword = searchModel.Keyword, SelectedSearchOption = searchModel.SearchOption });
            wordsSearchResult.ISearch = searchFactory.ISearch;
            return View(wordsSearchResult);
        }

        [HttpPost]
        public ActionResult SearchBySelectedWords(List<QuranWords> selectedWords, QuranTextType quranTextSearchType)
        {
            SetSession("SelectedWordsListSession", selectedWords);
            return Json(new { redirectToUrl = Url.Action("Search", "Home", new { SearchByWords = true, quranTextSearchType }) });
        }

        [HttpPost]
        public ActionResult ChangeSearchResultTextType(List<int> versesIds, QuranTextType quranTextType)
        {
            var verses = new List<Quran>();

            foreach (var id in versesIds)
            {
                var verse = DbContext.Quran.Single(x => x.ID == id);
                var verseText = verse.GetPropValue(quranTextType.ToString()) as string;
                verses.Add(new Quran { ID = verse.ID, AyahText = verseText });
            }

            return Json(new { verses });
        }

        [HttpPost]
        public ActionResult ChangeTafsir(int verseId, Tafsirs tafsir)
        {
            var tafsirText = (DbContext.Tafsirs.Single(x => x.ID == verseId).GetPropValue(tafsir.ToString()) as string);

            return Json(new { tafsirText });
        }

        [HttpGet("LogicsSearch")]
        public ActionResult LogicsSearch()
        {
            var logicsSearchModel = new LogicsSearchViewModel
            {
                SearchOptions = "[ \n"
            };
            foreach (var searchType in Enum.GetValues(typeof(SearchOptions)))
            {
                SearchOptions searchOption = (SearchOptions)Enum.Parse(typeof(SearchOptions), searchType.ToString());
                logicsSearchModel.SearchOptions += $"'{searchOption.GetDisplayName()}',{System.Environment.NewLine}";
            }
            logicsSearchModel.SearchOptions += "]";
            ViewBag.Message = "Your application description page.";

            return View(logicsSearchModel);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult LogicsSearchAjaxPost([FromBody] System.Text.Json.JsonElement logicsSearchSettingJson)
        {
            var str = logicsSearchSettingJson.ToString();
            str = (JsonConvert.DeserializeObject<dynamic>(str).logicsSearchSetting).ToString();
            var logicsSearchSetting = JsonConvert.DeserializeObject<LogicsSearchSetting>(str);
            TempData.Put("logicsSearchSetting", logicsSearchSetting);
            return Json(new { redirectToUrl = Url.Action("Search", "Home", new RouteValueDictionary(new SearchPostViewModel { LogicsSearch = true })) });
        }

        public List<string> GetAyahList(QuranTextType quranTextType)
        {
            return DbContext.Quran
             .Select(quranTextType.ToString())
             .ToDynamicList().Cast<string>().ToList();
        }

        [HttpPost]
        public JsonResult AutoComplete(string keyword, QuranTextType quranTextType)
        {
            if (!SessionExist("AyahAutoComplete"))
            {
                SetSession("AyahAutoComplete", GetAyahList(quranTextType));
            }
            var ayahList = GetSession<List<string>>("AyahAutoComplete");
            var searchResult = (from t in ayahList where t.Contains(keyword) select t).Take(5);
            return Json(searchResult);
        }

        [HttpPost]
        public JsonResult ChangeAutoComplete(QuranTextType quranTextType)
        {
            SetSession("AyahAutoComplete", GetAyahList(quranTextType));
            return new JsonResult(null);
        }
        string GetKeywords(LogicsSearchSetting logicsSearchSetting)
        {
            string keywords = "";
            foreach (var k in logicsSearchSetting.SelectedOptions1)
                keywords += " " + k.Keyword;
            foreach (var k in logicsSearchSetting.SelectedOptions2)
                keywords += " " + k.Keyword;
            return keywords.Trim();
        }
    }
}