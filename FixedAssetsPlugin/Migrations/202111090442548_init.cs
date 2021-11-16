namespace FixedAssetsPlugin.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FixedAssets",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 50),
                        State = c.Int(nullable: false),
                        Name = c.String(maxLength: 100),
                        ModelName = c.String(maxLength: 50),
                        UnitName = c.String(maxLength: 50),
                        Location = c.Int(nullable: false),
                        PrincipalName = c.String(maxLength: 50),
                        PrincipalPhone = c.String(maxLength: 50),
                        Register = c.DateTime(nullable: false),
                        LastCheck = c.DateTime(nullable: false),
                        Count = c.Int(nullable: false),
                        From = c.String(maxLength: 200),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelTime = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FixedAssetsChecks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FixedAssetsCode = c.String(maxLength: 50),
                        OldCount = c.Int(nullable: false),
                        NewCount = c.Int(nullable: false),
                        OldState = c.Int(nullable: false),
                        NewState = c.Int(nullable: false),
                        OldLocation = c.Int(nullable: false),
                        NewLocation = c.Int(nullable: false),
                        OldPrincipalName = c.String(maxLength: 50),
                        NewPrincipalName = c.String(maxLength: 50),
                        OldPrincipalPhone = c.String(maxLength: 50),
                        NewPrincipalPhone = c.String(maxLength: 50),
                        StaffId = c.String(maxLength: 50),
                        StaffName = c.String(maxLength: 50),
                        Check = c.DateTime(nullable: false),
                        Remark = c.String(maxLength: 200),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.Staffs",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 50),
            //            Name = c.String(maxLength: 50),
            //            QuickCode = c.String(maxLength: 200),
            //            IdCard = c.String(maxLength: 50),
            //            Sex = c.Int(nullable: false),
            //            Address = c.String(maxLength: 500),
            //            NowAddress = c.String(maxLength: 500),
            //            Phone = c.String(maxLength: 50),
            //            QQ = c.String(maxLength: 100),
            //            JobPostId = c.Int(nullable: false),
            //            IsDel = c.Boolean(nullable: false),
            //            DelUser = c.Int(nullable: false),
            //            DelTime = c.DateTime(nullable: false),
            //            Register = c.DateTime(nullable: false),
            //            Creator = c.Int(nullable: false),
            //            CreateTime = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.SysDics",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Name = c.String(maxLength: 100),
            //            QuickCode = c.String(maxLength: 100),
            //            ParentCode = c.String(maxLength: 100),
            //            Content = c.String(maxLength: 500),
            //            Creater = c.Int(nullable: false),
            //            CreateTime = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SysDics");
            DropTable("dbo.Staffs");
            DropTable("dbo.FixedAssetsChecks");
            DropTable("dbo.FixedAssets");
        }
    }
}
