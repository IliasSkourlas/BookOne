namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAssociatedRequestInBookCirculationModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookCirculations", "RequestForThisCirculation_BookRequestId", c => c.Int());
            CreateIndex("dbo.BookCirculations", "RequestForThisCirculation_BookRequestId");
            AddForeignKey("dbo.BookCirculations", "RequestForThisCirculation_BookRequestId", "dbo.BookRequests", "BookRequestId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookCirculations", "RequestForThisCirculation_BookRequestId", "dbo.BookRequests");
            DropIndex("dbo.BookCirculations", new[] { "RequestForThisCirculation_BookRequestId" });
            DropColumn("dbo.BookCirculations", "RequestForThisCirculation_BookRequestId");
        }
    }
}
