namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReplacedReactionIdInReactionModel : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Reactions");
            AlterColumn("dbo.Reactions", "ReactionId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Reactions", "ReactionId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Reactions");
            AlterColumn("dbo.Reactions", "ReactionId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Reactions", "ReactionId");
        }
    }
}
