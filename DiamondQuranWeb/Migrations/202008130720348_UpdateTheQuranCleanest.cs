namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UpdateTheQuranCleanest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Qurans", "AyahTextUthmaniCleanest", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Qurans", "AyahTextUthmaniCleanest");
        }
    }
}
