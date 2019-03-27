namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCarrierUsernameInBookModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "CarrierUsername", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Books", "CarrierUsername");
        }
    }
}
