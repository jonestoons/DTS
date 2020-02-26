namespace DocumentTracking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _28_Sep_2018_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "Subject", c => c.String(nullable: false));
            AlterColumn("dbo.Documents", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Documents", "Description", c => c.String());
            DropColumn("dbo.Documents", "Subject");
        }
    }
}
