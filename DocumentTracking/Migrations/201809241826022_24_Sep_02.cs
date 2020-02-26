namespace DocumentTracking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _24_Sep_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "isReset", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "isReset");
        }
    }
}
