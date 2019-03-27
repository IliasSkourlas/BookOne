namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReplacedCarrierUsernameInBookModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "Carrier_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Books", "Carrier_Id");
            AddForeignKey("dbo.Books", "Carrier_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Books", "CarrierUsername");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Books", "CarrierUsername", c => c.String());
            DropForeignKey("dbo.Books", "Carrier_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Books", new[] { "Carrier_Id" });
            DropColumn("dbo.Books", "Carrier_Id");
        }
    }
}
