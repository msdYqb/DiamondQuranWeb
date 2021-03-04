namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddUserFavorites : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FavoritesLists", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.FavoritesLists", "User_Id");
            AddForeignKey("dbo.FavoritesLists", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FavoritesLists", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.FavoritesLists", new[] { "User_Id" });
            DropColumn("dbo.FavoritesLists", "User_Id");
        }
    }
}
