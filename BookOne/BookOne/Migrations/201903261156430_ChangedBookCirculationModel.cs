namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedBookCirculationModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BookCirculations", "RequestForThisCirculation_BookRequestId", "dbo.BookRequests");
            DropIndex("dbo.BookCirculations", new[] { "RequestForThisCirculation_BookRequestId" });
            AddColumn("dbo.BookCirculations", "RequestIdForThisCirculation", c => c.Int(nullable: false));
            DropColumn("dbo.BookCirculations", "RequestForThisCirculation_BookRequestId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BookCirculations", "RequestForThisCirculation_BookRequestId", c => c.Int());
            DropColumn("dbo.BookCirculations", "RequestIdForThisCirculation");
            CreateIndex("dbo.BookCirculations", "RequestForThisCirculation_BookRequestId");
            AddForeignKey("dbo.BookCirculations", "RequestForThisCirculation_BookRequestId", "dbo.BookRequests", "BookRequestId");
        }
    }
}
