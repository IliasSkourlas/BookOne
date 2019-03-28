namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReplacedGiverAndReceiverIdInReactionModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reactions", "ActionGiver_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reactions", "ActionReceiver_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Reactions", new[] { "ActionGiver_Id" });
            DropIndex("dbo.Reactions", new[] { "ActionReceiver_Id" });
            AddColumn("dbo.Reactions", "ActionGiverId", c => c.String());
            AddColumn("dbo.Reactions", "ActionReceiverId", c => c.String());
            DropColumn("dbo.Reactions", "ActionGiver_Id");
            DropColumn("dbo.Reactions", "ActionReceiver_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reactions", "ActionReceiver_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Reactions", "ActionGiver_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.Reactions", "ActionReceiverId");
            DropColumn("dbo.Reactions", "ActionGiverId");
            CreateIndex("dbo.Reactions", "ActionReceiver_Id");
            CreateIndex("dbo.Reactions", "ActionGiver_Id");
            AddForeignKey("dbo.Reactions", "ActionReceiver_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Reactions", "ActionGiver_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
