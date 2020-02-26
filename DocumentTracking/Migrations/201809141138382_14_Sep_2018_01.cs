namespace DocumentTracking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _14_Sep_2018_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditTrails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        TDate = c.DateTime(nullable: false),
                        Action = c.Int(nullable: false),
                        MainID = c.String(),
                        RecordID = c.String(),
                        Module = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DeptId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.DeptId);
            
            CreateTable(
                "dbo.Units",
                c => new
                    {
                        UnitId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        DeptID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UnitId)
                .ForeignKey("dbo.Departments", t => t.DeptID, cascadeDelete: true)
                .Index(t => t.DeptID);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileId = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        UnitId = c.Int(nullable: false),
                        FileTypeId = c.Int(nullable: false),
                        UserID = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FileTypes", t => t.FileTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Units", t => t.UnitId, cascadeDelete: true)
                .Index(t => t.UnitId)
                .Index(t => t.FileTypeId);
            
            CreateTable(
                "dbo.FileTypes",
                c => new
                    {
                        FileTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.FileTypeId);
            
            CreateTable(
                "dbo.DocFileCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UnitId = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Units", t => t.UnitId, cascadeDelete: true)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.DocumentMovements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PrefID = c.String(),
                        Description = c.String(nullable: false),
                        FileType = c.Int(nullable: false),
                        Source = c.String(),
                        Destination = c.String(),
                        DateCreated = c.DateTime(),
                        DateStatus = c.DateTime(),
                        Status = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Linked = c.Boolean(nullable: false),
                        OldId = c.String(),
                        LinkDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmpID = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Unit = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.DocFileCodes", "UnitId", "dbo.Units");
            DropForeignKey("dbo.Documents", "UnitId", "dbo.Units");
            DropForeignKey("dbo.Documents", "FileTypeId", "dbo.FileTypes");
            DropForeignKey("dbo.Units", "DeptID", "dbo.Departments");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.DocFileCodes", new[] { "UnitId" });
            DropIndex("dbo.Documents", new[] { "FileTypeId" });
            DropIndex("dbo.Documents", new[] { "UnitId" });
            DropIndex("dbo.Units", new[] { "DeptID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.DocumentMovements");
            DropTable("dbo.DocFileCodes");
            DropTable("dbo.FileTypes");
            DropTable("dbo.Documents");
            DropTable("dbo.Units");
            DropTable("dbo.Departments");
            DropTable("dbo.Companies");
            DropTable("dbo.AuditTrails");
        }
    }
}
