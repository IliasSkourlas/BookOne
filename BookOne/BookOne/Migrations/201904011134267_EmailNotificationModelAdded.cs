namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailNotificationModelAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailNotifications",
                c => new
                    {
                        EmailNotificationId = c.Int(nullable: false, identity: true),
                        EmailAddress = c.String(nullable: false),
                        ReceivedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EmailNotificationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EmailNotifications");
        }
    }
}
