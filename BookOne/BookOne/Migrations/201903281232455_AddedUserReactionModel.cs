namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserReactionModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserReactions",
                c => new
                    {
                        UserReactionId = c.Int(nullable: false, identity: true),
                        ActionGiverId = c.String(),
                        ActionReceiverId = c.String(),
                        Choice = c.Int(nullable: false),
                        ReceivedOn = c.DateTime(nullable: false),
                        CirculationForThisReaction_BookCirculationId = c.Int(),
                    })
                .PrimaryKey(t => t.UserReactionId)
                .ForeignKey("dbo.BookCirculations", t => t.CirculationForThisReaction_BookCirculationId)
                .Index(t => t.CirculationForThisReaction_BookCirculationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserReactions", "CirculationForThisReaction_BookCirculationId", "dbo.BookCirculations");
            DropIndex("dbo.UserReactions", new[] { "CirculationForThisReaction_BookCirculationId" });
            DropTable("dbo.UserReactions");
        }
    }
}
