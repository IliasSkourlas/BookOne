namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReplacedCirculationIdInReactionModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reactions", "CirculationForThisReaction_BookCirculationId", "dbo.BookCirculations");
            DropIndex("dbo.Reactions", new[] { "CirculationForThisReaction_BookCirculationId" });
            AddColumn("dbo.Reactions", "CirculationIdForThisReaction", c => c.Int(nullable: false));
            DropColumn("dbo.Reactions", "CirculationForThisReaction_BookCirculationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reactions", "CirculationForThisReaction_BookCirculationId", c => c.Int());
            DropColumn("dbo.Reactions", "CirculationIdForThisReaction");
            CreateIndex("dbo.Reactions", "CirculationForThisReaction_BookCirculationId");
            AddForeignKey("dbo.Reactions", "CirculationForThisReaction_BookCirculationId", "dbo.BookCirculations", "BookCirculationId");
        }
    }
}
