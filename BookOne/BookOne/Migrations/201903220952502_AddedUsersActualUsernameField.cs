namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUsersActualUsernameField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ActualUsername", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ActualUsername");
        }
    }
}
