namespace DocumentTracking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20_Jan_2019_001 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Documents", "Subject", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Documents", "Subject", c => c.String(nullable: false));
        }
    }
}
