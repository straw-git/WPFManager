namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CoreSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaxLogCount = c.Int(nullable: false),
                        PluginsUpdateBaseUrl = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Emails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FromId = c.Int(nullable: false),
                        NoticeType = c.Int(nullable: false),
                        TargetId = c.String(),
                        Content = c.String(),
                        IsRead = c.Boolean(nullable: false),
                        SendTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogType = c.Int(nullable: false),
                        LogStr = c.String(),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ModulePages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PluginsId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        PageName = c.String(maxLength: 50),
                        PagePath = c.String(maxLength: 500),
                        Icon = c.String(maxLength: 20),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        DepartmentName = c.String(),
                        UserName = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Plugins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        DLLName = c.String(maxLength: 100),
                        LogoImage = c.String(maxLength: 200),
                        WebDownload = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                        ConnectionName = c.String(),
                        ConnectionString = c.String(),
                        UpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PluginsModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PluginsId = c.Int(nullable: false),
                        ModuleName = c.String(maxLength: 50),
                        Icon = c.String(maxLength: 20),
                        Order = c.Int(nullable: false),
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
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Pwd = c.String(maxLength: 100),
                        RoleId = c.Int(nullable: false),
                        CanLogin = c.Boolean(nullable: false),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelTime = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserNotices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        NoticeId = c.Int(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        ReadTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserPlugins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        IncreasePages = c.String(),
                        DecrementPages = c.String(),
                        UpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserPlugins");
            DropTable("dbo.UserNotices");
            DropTable("dbo.Users");
            DropTable("dbo.RolePlugins");
            DropTable("dbo.Roles");
            DropTable("dbo.PluginsModules");
            DropTable("dbo.Plugins");
            DropTable("dbo.Notices");
            DropTable("dbo.ModulePages");
            DropTable("dbo.Logs");
            DropTable("dbo.Emails");
            DropTable("dbo.CoreSettings");
        }
    }
}
