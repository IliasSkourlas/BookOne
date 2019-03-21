namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeChangesWithTheEntities2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BookCirculations", "Owner_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.BookRequests", "BookOwner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.BookCirculations", new[] { "Owner_Id" });
            DropIndex("dbo.BookRequests", new[] { "BookOwner_Id" });
            AddColumn("dbo.BookRequests", "RequestStatus", c => c.Int(nullable: false));
            DropColumn("dbo.BookCirculations", "Owner_Id");
            DropColumn("dbo.BookRequests", "BookOwner_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BookRequests", "BookOwner_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.BookCirculations", "Owner_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.BookRequests", "RequestStatus");
            CreateIndex("dbo.BookRequests", "BookOwner_Id");
            CreateIndex("dbo.BookCirculations", "Owner_Id");
            AddForeignKey("dbo.BookRequests", "BookOwner_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.BookCirculations", "Owner_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
