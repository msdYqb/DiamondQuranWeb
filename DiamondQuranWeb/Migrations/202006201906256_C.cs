namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class C : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Qurans", newName: "QuranCleaners");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.QuranCleaners", newName: "Qurans");
        }
    }
}
