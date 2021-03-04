namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class updateCleanNormalClean : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Qurans", "NormalClean", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Qurans", "NormalClean");
        }
    }
}
