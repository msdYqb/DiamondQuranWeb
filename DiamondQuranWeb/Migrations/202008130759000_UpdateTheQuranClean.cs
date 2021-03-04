namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UpdateTheQuranClean : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Qurans", "AyahText", "NormalCleanest");
            RenameColumn("dbo.Qurans", "AyahTextEnhanced", "NormalEnhanced");
            RenameColumn("dbo.Qurans", "AyahTextUthmani", "UthmaniFull");
            RenameColumn("dbo.Qurans", "AyahTextUthmaniClean", "UthmaniClean");
            RenameColumn("dbo.Qurans", "AyahTextUthmaniCleanest", "UthmaniCleanest");
        }

        public override void Down()
        {
            RenameColumn("dbo.Qurans", "AyahText", "NormalCleanest");
            RenameColumn("dbo.Qurans", "AyahTextEnhanced", "NormalEnhanced");
            RenameColumn("dbo.Qurans", "AyahTextUthmani", "UthmaniFull");
            RenameColumn("dbo.Qurans", "AyahTextUthmaniClean", "UthmaniClean");
            RenameColumn("dbo.Qurans", "AyahTextUthmaniCleanest", "UthmaniCleanest");
        }
    }
}
