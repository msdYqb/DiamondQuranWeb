using DiamondQuranWeb.Data;
using DiamondQuranWeb.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DiamondQuranWeb.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IWebHostEnvironment Environment;
        protected ApplicationDbContext DbContext { get; set; }
        protected readonly UserManager<IdentityUser> _userManager;
        public string CurrentUserId { get => _userManager.GetUserId(User); }
        protected async Task<IdentityUser> CurrentUserAsync()
        {
            //_userManager.GetUserAsync(HttpContext.User);
            //always use same dbcontext to retrieve user
            return await DbContext.Users.SingleAsync(x => x.Id == CurrentUserId);
        }
        public BaseController(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager, IWebHostEnvironment environment)
        {
            //DbContext = new ApplicationDbContext(Constants.DbContextOptions);
            DbContext = applicationDbContext;
            _userManager = userManager;
            Environment = environment;
        }
        protected void SetSession(string key, object data)
        {
            var str = JsonConvert.SerializeObject(data);
            HttpContext.Session.SetString(key, str);
        }
        protected T GetSession<T>(string key)
        {
            T obj = default;
            var str = HttpContext.Session.GetString(key);
            if (!string.IsNullOrWhiteSpace(str))
                obj = JsonConvert.DeserializeObject<T>(str);
            return obj;
        }

        protected bool SessionExist(string key) => string.IsNullOrWhiteSpace(HttpContext.Session.GetString(key)) ? false : true;

        protected override void Dispose(bool disposing)
        {
            //data.Dispose();
            //base.Dispose(disposing);
        }
    }
}