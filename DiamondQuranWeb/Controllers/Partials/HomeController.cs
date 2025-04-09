using DiamondQuranWeb.Models;
using DiamondQuranWeb.SearchEngine.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DiamondQuranWeb.Data;

namespace DiamondQuranWeb.Controllers
{
    public partial class HomeController : BaseController
    {
        public HomeController(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager, IWebHostEnvironment environment) : base(applicationDbContext,userManager, environment)
        {
        }
        [HttpGet("Quran")]
        public IActionResult Quran(QuranViewModel quranVm)
        {
            string path = Path.Combine(this.Environment.WebRootPath, "files/quranIndex.txt");
            var quranIndexStr = System.IO.File.ReadAllText(path);
            quranVm.QuranIndex = JsonConvert.DeserializeObject<List<QuranIndex>>(quranIndexStr);
            quranVm.CurrentDomain = Request.Scheme +"://" + Request.Host.Value;
            if (quranVm.SurahNumber != 0)
                quranVm.NavBySearch = true;

            var x = new List<QuranIndex>();
            x.Add(new QuranIndex { SurahName = "الفاتحه", SurahNumber = 1, VersesAmount = 3 });
            var QuranViewModel = new QuranViewModel
            {
                QuranIndex = x,
                CurrentDomain = "quranVm.CurrentDomain",
                SurahNumber = 1,
                AyahNumber = 1,
                NavBySearch = false
            };
            return View(quranVm);
        }

        [HttpGet("GetQuran")]
        public async Task<IActionResult> GetQuran(int currentSurah, int currentAyah, short page, bool navByPage)
        {
            if (navByPage)
                return Json(DbContext.Quran.Where(x => x.Page == page).ToList());
            var currentPage = DbContext.Quran.Single(x => x.SurahNumber == currentSurah && x.AyahNumber == currentAyah).Page;
            var verses = DbContext.Quran.Where(x => x.Page == currentPage).ToList();
            var versesJson = JsonConvert.SerializeObject(verses);
            return Content(versesJson);
        }

        [HttpGet]
        public async Task<ActionResult> GetTafsir(int ayahId)
        {
            await Task.Delay(1000);
            var tafsir = DbContext.Tafsirs.First(x => x.ID == ayahId).Saady;

            return new JsonResult(new { ayahId, tafsir });
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetComment(int ayahId)
        {
            var quran = DbContext.Quran.Single(x => x.ID == ayahId);
            var comment = DbContext.Comments.SingleOrDefault(x => x.User.Id == CurrentUserId && x.Quran.ID == quran.ID)?.Comment?.ToString();
            return new JsonResult(new { ayahId, comment });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddOrEditComment(int ayahId, string commentText)
        {
            var msg = "";
            if (string.IsNullOrWhiteSpace(commentText))
                msg = "خطا!";
            else
                try
                {
                    var quran = DbContext.Quran.Single(x => x.ID == ayahId);
                    var comment = DbContext.Comments.SingleOrDefault(x => x.User.Id == CurrentUserId && x.Quran.ID == quran.ID);
                    if (comment == null)
                    {
                        var currentUser = await CurrentUserAsync();
                        DbContext.Comments.Add(new Comments { User = currentUser, Quran = quran, Comment = commentText });
                        msg = "تم اضافة التعليق";
                    }
                    else
                    {
                        comment.Comment = commentText;
                        msg = "تم التعديل على التعليق";
                    }
                    var saveChangesNumber = DbContext.SaveChanges();
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
                var quran = DbContext.Quran.Single(x => x.ID == ayahId);
                var comment = DbContext.Comments.SingleOrDefault(x => x.User.Id == CurrentUserId && x.Quran.ID == quran.ID);
                if (comment == null)
                    msg = "لايوجد تعليق";
                else
                {
                    DbContext.Comments.Remove(comment);
                    var saveChangesNumber = DbContext.SaveChanges();
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
            var favoritesList = DbContext.FavoritesList.Where(x => x.User.Id == CurrentUserId).ToList();
            foreach (var list in favoritesList)
            {
                favoritesString += list.ListName + "-" + list.ID.ToString() + ",".Trim();
            }
            favoritesString = favoritesString.Trim();
            return new JsonResult(new { ayahId, favoritesString });
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddFavorite(int ayahId, int listId)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var favoriteList = DbContext.FavoritesList.Single(x => x.User.Id == CurrentUserId && x.ID == listId);
                var verse = DbContext.Quran.Single(x => x.ID == ayahId);
                DbContext.Favorites.Add(new Favorites { FavoriteList = favoriteList, Quran = verse });
                if (DbContext.SaveChanges() > 0) success = true;
                else success = false;
            }
            catch
            {
                success = false;
            }

            return new JsonResult(new { success });
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
                var favoriteList = DbContext.FavoritesList.Single(x => x.User.Id == CurrentUserId && x.ID == listId);
                foreach (var id in versesId)
                {
                    var verse = DbContext.Quran.Single(x => x.ID == id);
                    DbContext.Favorites.Add(new Favorites { FavoriteList = favoriteList, Quran = verse });
                }
                if (DbContext.SaveChanges() > 0) success = true;
                else success = false;
            }
            catch
            {
                success = false;
            }

            return new JsonResult(new { success });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> RemoveFavorite(int favoriteId, int favoriteListId)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var favorite = DbContext.Favorites.Single(x => x.FavoriteList.ID == favoriteListId && x.ID == favoriteId);
                DbContext.Favorites.Remove(favorite);
                if (DbContext.SaveChanges() > 0) success = true;
                else success = false;
            }
            catch (Exception ex)
            {
                success = false;
            }

            return new JsonResult(new { favoriteId, success });
        }
        [HttpPost]
        public async Task<ActionResult> DeleteFavoriteList(int favoriteId)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var favoriteList = DbContext.FavoritesList.Single(x => x.User.Id == CurrentUserId && x.ID == favoriteId);
                var favorites = DbContext.Favorites.Where(x => x.FavoriteList.ID == favoriteList.ID);
                DbContext.Favorites.RemoveRange(favorites);
                DbContext.FavoritesList.Remove(favoriteList);
                if (DbContext.SaveChanges() > 0) success = true;
                else success = false;
            }
            catch
            {
                success = false;
            }

            return new JsonResult(new { favoriteId, success });
        }
        [HttpGet("About")]
        public IActionResult About()
        {
            return View();
        }
    }
}