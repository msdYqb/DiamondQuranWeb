namespace DiamondQuranWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UpdatePseudo : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Qurans", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Qurans", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
    }
}
