namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class tafsir : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tafsirs",
                c => new
                    {
                        ID = c.Short(nullable: false, identity: true),
                        Saady = c.String(),
                        Baghawy = c.String(),
                        QuranCleaner_ID = c.Short(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.QuranCleaners", t => t.QuranCleaner_ID)
                .Index(t => t.QuranCleaner_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tafsirs", "QuranCleaner_ID", "dbo.QuranCleaners");
            DropIndex("dbo.Tafsirs", new[] { "QuranCleaner_ID" });
            DropTable("dbo.Tafsirs");
        }
    }
}
