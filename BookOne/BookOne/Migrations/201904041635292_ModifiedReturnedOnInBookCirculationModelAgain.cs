namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedReturnedOnInBookCirculationModelAgain : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BookCirculations", "ReturnedOn", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BookCirculations", "ReturnedOn", c => c.String());
        }
    }
}
