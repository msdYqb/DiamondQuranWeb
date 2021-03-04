using DiamondQuranWeb.Models;
using DiamondQuranWeb.SearchEngine.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DiamondQuranWeb.Controllers
{
    public partial class AccountController : BaseController
    {
        [Authorize]
        [HttpGet]
        public ActionResult Favorites(int favoriteListId)
        {
            FavoritesViewModel favoritesViewModel = new FavoritesViewModel
            {
                Favorites = (from favorite in data.Favorites
                             where favorite.FavoriteList.ID == favoriteListId
                             select favorite).ToList(),
                ListName = data.FavoritesList.Single(x => x.ID == favoriteListId && x.User.Id == CurrentUser.Id).ListName
            };

            return View(favoritesViewModel);
        }

        [Authorize]
        [ActionName("Bookmarks")]
        [HttpGet]
        public ActionResult Bookmarks_Get()
        {
            BookmarksViewModel bookmarks = new BookmarksViewModel
            {
                FavoritesList = data.FavoritesList.Where(x => x.User.Id == CurrentUser.Id).ToList(),
                Comments = data.Comments.Where(x => x.User.Id == CurrentUser.Id).ToList(),
            };

            return View(bookmarks);
        }

        [Authorize]
        [ActionName("Bookmarks")]
        [HttpPost]
        public ActionResult Bookmarks_Post(BookmarksViewModel bookmark)
        {
            if (bookmark.FavoriteList.Contains(','))
                return RedirectToAction("Bookmarks");
            if (data.FavoritesList.SingleOrDefault(x => x.ListName == bookmark.FavoriteList) != null)
            {
                return RedirectToAction("Bookmarks");
            }
            data.FavoritesList.Add(new FavoritesList { User = CurrentUser, ListName = bookmark.FavoriteList });
            data.SaveChanges();

            return RedirectToAction("Bookmarks");
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

        [HttpPost]
        public async Task<ActionResult> EditFavoriteList(int favoriteId, string listName)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var favoriteList = data.FavoritesList.Single(x => x.User.Id == CurrentUser.Id && x.ID == favoriteId);
                favoriteList.ListName = listName;
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
        public async Task<ActionResult> EditComment(int commentId, string comment)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var comments = data.Comments.Single(x => x.User.Id == CurrentUser.Id && x.ID == commentId);
                comments.Comment = comment;
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
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            await Task.Delay(1000);
            var success = new bool();
            try
            {
                var comment = data.Comments.Single(x => x.User.Id == CurrentUser.Id && x.ID == commentId);
                data.Comments.Remove(comment);
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
                Data = new { commentId, success }
            };
        }
    }
}