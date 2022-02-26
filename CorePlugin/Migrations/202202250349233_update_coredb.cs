namespace CorePlugin.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_coredb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(maxLength: 50),
                        LogInfo = c.String(maxLength: 200),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Plugins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 50),
                        DLLName = c.String(maxLength: 100),
                        ModuleTitles = c.String(maxLength: 300),
                        ModuleFolderPaths = c.String(maxLength: 500),
                        ModuleIcon = c.String(maxLength: 300),
                        UpdateUrl = c.String(maxLength: 200),
                        DBModelUrl = c.String(maxLength: 500),
                        Order = c.Int(nullable: false),
                        UpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PluginsPages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PluginsId = c.Int(nullable: false),
                        ModuleTitle = c.String(maxLength: 50),
                        ModuleFolderPath = c.String(maxLength: 100),
                        PageTitle = c.String(maxLength: 50),
                        PagePath = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelUserName = c.String(maxLength: 50),
                        DelTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RolePlugins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        Pages = c.String(maxLength: 200),
                        UpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SysDics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        QuickCode = c.String(maxLength: 100),
                        ParentCode = c.String(maxLength: 100),
                        Content = c.String(maxLength: 500),
                        Creater = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Pwd = c.String(maxLength: 100),
                        RoleId = c.Int(nullable: false),
                        StaffId = c.String(maxLength: 50),
                        CanLogin = c.Boolean(nullable: false),
                        LoadPluginsType = c.Int(nullable: false),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelTime = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserPlugins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Pages = c.String(),
                        UpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserPlugins");
            DropTable("dbo.Users");
            DropTable("dbo.SysDics");
            DropTable("dbo.RolePlugins");
            DropTable("dbo.Roles");
            DropTable("dbo.PluginsPages");
            DropTable("dbo.Plugins");
            DropTable("dbo.Logs");
        }
    }
}
