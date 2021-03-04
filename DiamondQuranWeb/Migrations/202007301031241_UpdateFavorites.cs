namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UpdateFavorites : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Comment = c.String(),
                        Quran_ID = c.Short(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.QuranCleaners", t => t.Quran_ID)
                .Index(t => t.Quran_ID);
            
            CreateTable(
                "dbo.Favorites",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FavoriteList_ID = c.Int(),
                        Quran_ID = c.Short(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.FavoritesLists", t => t.FavoriteList_ID)
                .ForeignKey("dbo.QuranCleaners", t => t.Quran_ID)
                .Index(t => t.FavoriteList_ID)
                .Index(t => t.Quran_ID);
            
            CreateTable(
                "dbo.FavoritesLists",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ListName = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Favorites", "Quran_ID", "dbo.QuranCleaners");
            DropForeignKey("dbo.Favorites", "FavoriteList_ID", "dbo.FavoritesLists");
            DropForeignKey("dbo.Comments", "Quran_ID", "dbo.QuranCleaners");
            DropIndex("dbo.Favorites", new[] { "Quran_ID" });
            DropIndex("dbo.Favorites", new[] { "FavoriteList_ID" });
            DropIndex("dbo.Comments", new[] { "Quran_ID" });
            DropTable("dbo.FavoritesLists");
            DropTable("dbo.Favorites");
            DropTable("dbo.Comments");
        }
    }
}
