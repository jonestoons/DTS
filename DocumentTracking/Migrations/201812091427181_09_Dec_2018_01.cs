namespace DocumentTracking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _09_Dec_2018_01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "OldFileId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "OldFileId");
        }
    }
}
