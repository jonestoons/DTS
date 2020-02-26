namespace DocumentTracking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _24_Sep_2018_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MsgSetups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Password = c.String(),
                        Port = c.String(),
                        Host = c.String(),
                        Backlog = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MsgSetups");
        }
    }
}
