namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBorrowerAsksToReturnBookInBookModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "BorrowerAskedToReturnThisBook", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Books", "BorrowerAskedToReturnThisBook");
        }
    }
}
