namespace DocumentTracking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _18_Sep_2018_01 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Companies", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Companies", "Description", c => c.String());
        }
    }
}
