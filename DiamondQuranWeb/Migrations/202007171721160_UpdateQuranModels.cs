namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UpdateQuranModels : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.QuranCleaners");
            AddColumn("dbo.QuranCleaners", "Page", c => c.Short(nullable: false));
            AddColumn("dbo.QuranCleaners", "AyahTextEnhanced", c => c.String());
            AddColumn("dbo.QuranCleaners", "AyahTextUthmani", c => c.String());
            AlterColumn("dbo.QuranCleaners", "ID", c => c.Short(nullable: false, identity: true));
            AlterColumn("dbo.QuranCleaners", "SurahNumber", c => c.Short(nullable: false));
            AlterColumn("dbo.QuranCleaners", "AyahNumber", c => c.Short(nullable: false));
            AddPrimaryKey("dbo.QuranCleaners", "ID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.QuranCleaners");
            AlterColumn("dbo.QuranCleaners", "AyahNumber", c => c.Int(nullable: false));
            AlterColumn("dbo.QuranCleaners", "SurahNumber", c => c.Int(nullable: false));
            AlterColumn("dbo.QuranCleaners", "ID", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.QuranCleaners", "AyahTextUthmani");
            DropColumn("dbo.QuranCleaners", "AyahTextEnhanced");
            DropColumn("dbo.QuranCleaners", "Page");
            AddPrimaryKey("dbo.QuranCleaners", "ID");
        }
    }
}
