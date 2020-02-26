namespace DocumentTracking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _28_Sep_2018_02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DocumentMovements", "Subject", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DocumentMovements", "Subject");
        }
    }
}
