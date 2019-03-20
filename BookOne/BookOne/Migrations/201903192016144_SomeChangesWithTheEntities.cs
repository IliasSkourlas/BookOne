namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeChangesWithTheEntities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "AvailabilityStatus", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Books", "AvailabilityStatus");
        }
    }
}
