using DiamondQuranWeb.Models;
using DiamondQuranWeb.SearchEngine.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiamondQuranWeb.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Quran> Quran { get; set; }
    public DbSet<Tafsirs> Tafsirs { get; set; }
    public DbSet<Comments> Comments { get; set; }
    public DbSet<FavoritesList> FavoritesList { get; set; }
    public DbSet<Favorites> Favorites { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
