using DiamondQuranWeb.Data;
using DiamondQuranWeb.Models;
using DiamondQuranWeb.SearchEngine.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiamondQuranWeb.Controllers
{
    public partial class AccountController : BaseController
    {
        public AccountController(ApplicationDbContext applicationDbContext,UserManager<IdentityUser> userManager, IWebHostEnvironment environment) : base(applicationDbContext, userManager, environment)
        {
        }

        [HttpGet("Account/IsAuthenticated")]
        public async Task<ActionResult> IsAuthenticated()
        {
            var isAuthenticated = User.Identity.IsAuthenticated;
            return Json(new { isAuthenticated });
        }

        [Authorize]
        [HttpGet("Account/Favorites")]
        public async Task<ActionResult> Favorites(int favoriteListId)
        {
            var currentUser = await CurrentUserAsync();
            var favList = DbContext.FavoritesList.Where(x => x.User == currentUser && x.ID == favoriteListId);
            if (favList == null) return NotFound();
            var favoritesViewModel = new FavoritesViewModel
            {
                Favorites = DbContext.Favorites.Where(x => x.FavoriteList.ID == favoriteListId).Include(x => x.Quran).ToList(),
                ListName = DbContext.FavoritesList.Single(x => x.ID == favoriteListId && x.User.Id == CurrentUserId).ListName
            };

            return View(favoritesViewModel);
        }

        [Authorize]
        [HttpGet("Account/Bookmarks")]
        public ActionResult Bookmarks()
        {
            BookmarksViewModel bookmarks = new()
            {
                FavoritesList = DbContext.FavoritesList.Where(x => x.User.Id == CurrentUserId).ToList(),
                Comments = DbContext.Comments.Where(x => x.User.Id == CurrentUserId).Include(x => x.Quran).ToList(),
            };

            return View(bookmarks);
        }

        [Authorize]
        [HttpPost("Account/Bookmarks")]
        public async Task<ActionResult> Bookmarks(BookmarksViewModel bookmark)
        {
            if (bookmark.FavoriteList.Contains(','))
                return RedirectToAction("Bookmarks");
            if (DbContext.FavoritesList.SingleOrDefault(x => x.ListName == bookmark.FavoriteList) != null)
            {
                return RedirectToAction("Bookmarks");
            }
            var currenctUser = await CurrentUserAsync();
            DbContext.FavoritesList.Add(new FavoritesList { User = currenctUser, ListName = bookmark.FavoriteList });
            DbContext.SaveChanges();

            return RedirectToAction("Bookmarks");
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

        [HttpPost]
        public async Task<ActionResult> EditFavoriteList(int favoriteId, string listName)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var favoriteList = DbContext.FavoritesList.Single(x => x.User.Id == CurrentUserId && x.ID == favoriteId);
                favoriteList.ListName = listName;
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
        public async Task<ActionResult> EditComment(int commentId, string comment)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var comments = DbContext.Comments.Single(x => x.User.Id == CurrentUserId && x.ID == commentId);
                comments.Comment = comment;
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
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var comment = DbContext.Comments.Single(x => x.User.Id == CurrentUserId && x.ID == commentId);
                DbContext.Comments.Remove(comment);
                if (DbContext.SaveChanges() > 0) success = true;
                else success = false;
            }
            catch
            {
                success = false;
            }

            return new JsonResult(new { commentId, success });
        }
    }
}