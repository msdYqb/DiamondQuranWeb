namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DbSetQuranUthmaniClean : DbMigration
    {
        public override void Up()
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.QuranUthmaniCleans");
        }
    }
}
