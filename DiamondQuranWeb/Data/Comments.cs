using DiamondQuranWeb.SearchEngine.Models;
using Microsoft.AspNetCore.Identity;

namespace DiamondQuranWeb.Models
{
    public class Comments
    {
        public int ID { get; set; }
        public virtual IdentityUser User { get; set; }
        public virtual Quran Quran { get; set; }
        public string Comment { get; set; }
    }
    public class FavoritesList
    {
        public int ID { get; set; }
        public virtual IdentityUser User { get; set; }
        public string ListName { get; set; }
    }
    public class Favorites
    {
        public int ID { get; set; }
        public virtual FavoritesList FavoriteList { get; set; }
        public virtual Quran Quran { get; set; }
    }
}