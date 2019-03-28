namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DroppedOlderReactionsTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reactions", "CirculationForThisReaction_BookCirculationId", "dbo.BookCirculations");
            DropIndex("dbo.Reactions", new[] { "CirculationForThisReaction_BookCirculationId" });
            DropTable("dbo.Reactions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Reactions",
                c => new
                    {
                        ReactionId = c.Int(nullable: false, identity: true),
                        ActionGiverId = c.String(),
                        ActionReceiverId = c.String(),
                        Choice = c.Int(nullable: false),
                        ReceivedOn = c.DateTime(nullable: false),
                        CirculationForThisReaction_BookCirculationId = c.Int(),
                    })
                .PrimaryKey(t => t.ReactionId);
            
            CreateIndex("dbo.Reactions", "CirculationForThisReaction_BookCirculationId");
            AddForeignKey("dbo.Reactions", "CirculationForThisReaction_BookCirculationId", "dbo.BookCirculations", "BookCirculationId");
        }
    }
}
