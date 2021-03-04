using DiamondQuranWeb.Models;
using DiamondQuranWeb.SearchEngine.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DiamondQuranWeb.Controllers
{
    public partial class HomeController : BaseController
    {

        public ApplicationUser CurrentUser
        {
            get
            {
                var userId = User.Identity.GetUserId();
                return data.Users.Single(x => x.Id == userId);
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public HomeController()
        {
        }

        [ActionName("Quran")]
        [HttpGet]
        public ActionResult Quran_Get(QuranViewModel quran)
        {
            if (quran.Page == 0)
                quran.Page = data.Quran.Single(x => x.SurahNumber == quran.SurahNumber && x.AyahNumber == (quran.AyahNumber == 0 ? 1 : quran.AyahNumber)).Page;
            if (TempData["SurahNumber"] != null) quran.SurahNumber = (short)TempData["SurahNumber"];
            else quran.SurahNumber = 1;
            if (TempData["AyahNumber"] != null) quran.AyahNumber = (short)TempData["AyahNumber"];

            (quran.SurahList, quran.AyahList) = GetQuranIndexList(quran.SurahNumber);

            return View(quran);
        }
        [ActionName("Quran")]
        [HttpPost]
        public ActionResult Quran_Post(QuranViewModel quran, string command)
        {
            if (command == "Next")
            {
                quran.Page = ++quran.Page;
                ChangeSurah();
            }
            else if (command == "Previous")
            {
                quran.Page = --quran.Page;
                ChangeSurah();
            }

            return RedirectToAction("Quran", "Home", new { quran.Page });
            void ChangeSurah()
            {
                TempData["SurahNumber"] = data.Quran.First(x => x.Page == quran.Page).SurahNumber;
            }
        }

        [HttpPost]
        public ActionResult ChangeSurah(QuranViewModel quran)
        {
            quran.Page = data.Quran.First(x => x.SurahNumber == quran.SurahNumber).Page;
            TempData["SurahNumber"] = quran.SurahNumber;
            return Json(new { redirectToUrl = Url.Action("Quran", "Home") + "?Page=" + quran.Page });
        }

        [HttpPost]
        public ActionResult ChangeAyah(QuranViewModel quran)
        {
            quran.Page = data.Quran.First(x => x.SurahNumber == quran.SurahNumber && x.AyahNumber == quran.AyahNumber).Page;
            TempData["SurahNumber"] = quran.SurahNumber;
            TempData["AyahNumber"] = quran.AyahNumber;
            return Json(new { redirectToUrl = Url.Action("Quran", "Home") + "?Page=" + quran.Page });
        }

        (List<SelectListItem> surahIndexList, List<SelectListItem> ayahIndexList) GetQuranIndexList(int selectedSurah)
        {
            if (Session["QuranIndexList"] == null)
            {
                var surahNameList = new List<string>();
                foreach (var ayah in data.Quran)
                {
                    var exist = surahNameList.SingleOrDefault(x => x == ayah.SurahName) == null ? false : true;
                    if (!exist)
                        surahNameList.Add(ayah.SurahName);
                }
                var quranIndex = "";
                foreach (var surahName in surahNameList)
                {
                    var surahIndex = data.Quran.Where(x => x.SurahName == surahName).First().SurahNumber;
                    var ayahIndex = data.Quran.Where(x => x.SurahName == surahName).OrderByDescending(x => x.AyahNumber).First().AyahNumber;
                    quranIndex += $"{surahName}-{surahIndex}-{ayahIndex},";
                }
                Session["QuranIndexList"] = quranIndex;
            }
            {
                var quranIndexStr = Session["QuranIndexList"].ToString();
                var surahIndexList = new List<SelectListItem>();
                var ayahIndexList = new List<SelectListItem>();
                foreach (var item in quranIndexStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var surahName = item.Split('-')[0];
                    var surahIndex = int.Parse(item.Split('-')[1]);
                    var ayahIndex = int.Parse(item.Split('-')[2]);
                    surahIndexList.Add(new SelectListItem { Text = surahName, Value = surahIndex.ToString() });
                    if (selectedSurah == surahIndex)
                    {
                        for (int i = 0; i < ayahIndex; i++)
                        {
                            ayahIndexList.Add(new SelectListItem { Text = (i + 1).ToString(), Value = (i + 1).ToString() });
                        }
                    }
                }
                return (surahIndexList, ayahIndexList);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetTafsir(int ayahId)
        {
            await Task.Delay(1000);
            var quranCleaner = data.Quran.First(x => x.ID == ayahId);
            var tafsir = data.Tafsirs.First(x => x.QuranCleaner.ID == quranCleaner.ID).Saady;

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { ayahId , tafsir }
            };
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetComment(int ayahId)
        {
            var quran = data.Quran.Single(x => x.ID == ayahId);
            var comment = data.Comments.SingleOrDefault(x => x.User.Id == CurrentUser.Id && x.Quran.ID == quran.ID)?.Comment?.ToString();
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { ayahId, comment }
            };
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddOrEditComment(int ayahId, string commentText)
        {
            var msg = "";
            if (string.IsNullOrWhiteSpace(commentText))
                msg = "خطا!";
            else
                try
                {
                    var quran = data.Quran.Single(x => x.ID == ayahId);
                    var comment = data.Comments.SingleOrDefault(x => x.User.Id == CurrentUser.Id && x.Quran.ID == quran.ID);
                    if (comment == null)
                    {
                        data.Comments.Add(new SearchEngine.Models.Comments { User = CurrentUser, Quran = quran, Comment = commentText });
                        msg = "تم اضافة التعليق";
                    }
                    else
                    {
                        comment.Comment = commentText;
                        msg = "تم التعديل على التعليق";
                    }
                    var saveChangesNumber = data.SaveChanges();
                    if (saveChangesNumber <= 0) msg = "خطا!";
                }
                catch (Exception ex)
                {
                    msg = "خطا!";
                }
            return Json(new { msg });
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteComment(int ayahId)
        {
            var msg = "";
            var success = new bool();
            try
            {
                var quran = data.Quran.Single(x => x.ID == ayahId);
                var comment = data.Comments.SingleOrDefault(x => x.User.Id == CurrentUser.Id && x.Quran.ID == quran.ID);
                if (comment == null)
                    msg = "لايوجد تعليق";
                else
                {
                    data.Comments.Remove(comment);
                    var saveChangesNumber = data.SaveChanges();
                    if (saveChangesNumber <= 0) msg = "خطا!";
                    else
                    {
                        msg = "تم مسح التعليق";
                        success = true;
                    }
                }
            }
            catch
            {
                msg = "خطا!";
            }

            return Json(new { msg, success });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetFavorites(int ayahId)
        {
            await Task.Delay(1000);
            var favoritesString = "";
            var favoritesList = data.FavoritesList.Where(x => x.User.Id == CurrentUser.Id).ToList();
            foreach (var list in favoritesList)
            {
                favoritesString += list.ListName + "-" + list.ID.ToString() + ",".Trim();
            }
            favoritesString = favoritesString.Trim();
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { ayahId , favoritesString }
            };
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddFavorite(int ayahId, int listId)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var favoriteList = data.FavoritesList.Single(x => x.User.Id == CurrentUser.Id && x.ID == listId);
                var verse = data.Quran.Single(x => x.ID == ayahId);
                data.Favorites.Add(new SearchEngine.Models.Favorites { FavoriteList = favoriteList, Quran = verse });
                if (data.SaveChanges() > 0) success = true;
                else success = false;
            }
            catch
            {
                success = false;
            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { success }
            };
        }

        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAllFavorite(List<int> versesId, int listId)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var favoriteList = data.FavoritesList.Single(x => x.User.Id == CurrentUser.Id && x.ID == listId);
                foreach (var id in versesId)
                {
                    var verse = data.Quran.Single(x => x.ID == id);
                    data.Favorites.Add(new Favorites { FavoriteList = favoriteList, Quran = verse });
                }
                if (data.SaveChanges() > 0) success = true;
                else success = false;
            }
            catch
            {
                success = false;
            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { success }
            };
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> RemoveFavorite(int favoriteId, int favoriteListId)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var favorite = data.Favorites.Single(x => x.FavoriteList.ID == favoriteListId && x.ID == favoriteId);
                data.Favorites.Remove(favorite);
                if (data.SaveChanges() > 0) success = true;
                else success = false;
            }
            catch (Exception ex)
            {
                success = false;
            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { favoriteId, success }
            };
        }
        [HttpPost]
        public async Task<ActionResult> DeleteFavoriteList(int favoriteId)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var favoriteList = data.FavoritesList.Single(x => x.User.Id == CurrentUser.Id && x.ID == favoriteId);
                var favorites = data.Favorites.Where(x => x.FavoriteList.ID == favoriteList.ID);
                data.Favorites.RemoveRange(favorites);
                data.FavoritesList.Remove(favoriteList);
                if (data.SaveChanges() > 0) success = true;
                else success = false;
            }
            catch
            {
                success = false;
            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { favoriteId, success }
            };
        }


        //
        // POST: /Home/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Main", "Home");
        }
    }
}