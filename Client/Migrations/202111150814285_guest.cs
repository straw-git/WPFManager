namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        QuickCode = c.String(maxLength: 200),
                        IdCard = c.String(maxLength: 50),
                        Sex = c.String(maxLength: 10),
                        Birthday = c.String(maxLength: 20),
                        Address = c.String(maxLength: 200),
                        AddressNow = c.String(maxLength: 200),
                        Phone = c.String(maxLength: 20),
                        CardNumber = c.String(maxLength: 50),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RechargeCount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MemberLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        LogPriceCount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PayOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 50),
                        Type = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelTime = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PayOrders");
            DropTable("dbo.MemberLevels");
            DropTable("dbo.Members");
        }
    }
}
