namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedCirculationInReactionModelAgain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reactions", "CirculationForThisReaction_BookCirculationId", c => c.Int());
            CreateIndex("dbo.Reactions", "CirculationForThisReaction_BookCirculationId");
            AddForeignKey("dbo.Reactions", "CirculationForThisReaction_BookCirculationId", "dbo.BookCirculations", "BookCirculationId");
            DropColumn("dbo.Reactions", "CirculationIdForThisReaction");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reactions", "CirculationIdForThisReaction", c => c.Int(nullable: false));
            DropForeignKey("dbo.Reactions", "CirculationForThisReaction_BookCirculationId", "dbo.BookCirculations");
            DropIndex("dbo.Reactions", new[] { "CirculationForThisReaction_BookCirculationId" });
            DropColumn("dbo.Reactions", "CirculationForThisReaction_BookCirculationId");
        }
    }
}
