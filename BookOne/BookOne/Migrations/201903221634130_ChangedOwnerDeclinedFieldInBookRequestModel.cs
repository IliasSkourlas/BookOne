namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedOwnerDeclinedFieldInBookRequestModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookRequests", "OwnerDeclined", c => c.Boolean(nullable: false));
            DropColumn("dbo.BookRequests", "ApprovedByOwner");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BookRequests", "ApprovedByOwner", c => c.Boolean(nullable: false));
            DropColumn("dbo.BookRequests", "OwnerDeclined");
        }
    }
}
