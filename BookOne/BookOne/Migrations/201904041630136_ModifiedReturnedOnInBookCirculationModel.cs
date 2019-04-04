namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedReturnedOnInBookCirculationModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BookCirculations", "ReturnedOn", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BookCirculations", "ReturnedOn", c => c.DateTime(nullable: false));
        }
    }
}
