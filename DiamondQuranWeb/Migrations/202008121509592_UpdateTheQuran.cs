namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UpdateTheQuran : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.QuranEnhanceds");
            DropTable("dbo.QuranUthmanis");
            DropTable("dbo.QuranUthmaniCleans");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.QuranUthmaniCleans",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SurahName = c.String(),
                        SurahNumber = c.Int(nullable: false),
                        AyahNumber = c.Int(nullable: false),
                        AyahText = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.QuranUthmanis",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SurahName = c.String(),
                        SurahNumber = c.Int(nullable: false),
                        AyahNumber = c.Int(nullable: false),
                        AyahText = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.QuranEnhanceds",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SurahName = c.String(),
                        SurahNumber = c.Int(nullable: false),
                        AyahNumber = c.Int(nullable: false),
                        AyahText = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
    }
}
