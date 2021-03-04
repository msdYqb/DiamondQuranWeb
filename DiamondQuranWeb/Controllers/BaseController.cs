using DiamondQuranWeb.Models;
using System.Web.Mvc;

namespace DiamondQuranWeb.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController()
        {
            data = new ApplicationDbContext();
        }

        protected ApplicationDbContext data { get; set; }

        protected override void Dispose(bool disposing)
        {
            data.Dispose();
            base.Dispose(disposing);
        }
    }
}