namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UpdateQuran : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.QuranCleaners", newName: "Qurans");
            AddColumn("dbo.Qurans", "AyahTextUthmaniClean", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Qurans", "AyahTextUthmaniClean");
            RenameTable(name: "dbo.Qurans", newName: "QuranCleaners");
        }
    }
}
