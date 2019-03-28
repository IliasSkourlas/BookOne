namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReplacedBookInReactionModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reactions", "ForBook_BookId", "dbo.Books");
            DropIndex("dbo.Reactions", new[] { "ForBook_BookId" });
            AddColumn("dbo.Reactions", "CirculationForThisReaction_BookCirculationId", c => c.Int());
            CreateIndex("dbo.Reactions", "CirculationForThisReaction_BookCirculationId");
            AddForeignKey("dbo.Reactions", "CirculationForThisReaction_BookCirculationId", "dbo.BookCirculations", "BookCirculationId");
            DropColumn("dbo.Reactions", "ForBook_BookId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reactions", "ForBook_BookId", c => c.Int());
            DropForeignKey("dbo.Reactions", "CirculationForThisReaction_BookCirculationId", "dbo.BookCirculations");
            DropIndex("dbo.Reactions", new[] { "CirculationForThisReaction_BookCirculationId" });
            DropColumn("dbo.Reactions", "CirculationForThisReaction_BookCirculationId");
            CreateIndex("dbo.Reactions", "ForBook_BookId");
            AddForeignKey("dbo.Reactions", "ForBook_BookId", "dbo.Books", "BookId");
        }
    }
}
