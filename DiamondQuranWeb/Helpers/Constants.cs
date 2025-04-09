using DiamondQuranWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace DiamondQuranWeb.Helpers
{
    public class Constants
    {
        public const string AppName = "القران الماسي";
        public static string ConStr { get; set; }
        public static DbContextOptions<ApplicationDbContext> DbContextOptions
        {
            get
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(ConStr);
                return optionsBuilder.Options;
            }
        }
    }
}
