namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addTafsirs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tafsirs", "Katheer", c => c.String());
            AddColumn("dbo.Tafsirs", "Qortoby", c => c.String());
            AddColumn("dbo.Tafsirs", "Tabary", c => c.String());
            AddColumn("dbo.Tafsirs", "Tanweer", c => c.String());
            AddColumn("dbo.Tafsirs", "Waseet", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tafsirs", "Waseet");
            DropColumn("dbo.Tafsirs", "Tanweer");
            DropColumn("dbo.Tafsirs", "Tabary");
            DropColumn("dbo.Tafsirs", "Qortoby");
            DropColumn("dbo.Tafsirs", "Katheer");
        }
    }
}
