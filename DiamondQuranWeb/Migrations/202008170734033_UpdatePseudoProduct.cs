namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UpdatePseudoProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Qurans", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Qurans", "Discriminator");
        }
    }
}
