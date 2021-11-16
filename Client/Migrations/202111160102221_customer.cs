namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
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
                        PromotionCode = c.String(maxLength: 10),
                        BePromotionCode = c.String(maxLength: 10),
                        IsMember = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        Creater = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerTemps",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CustomerId = c.Int(nullable: false),
                        Name = c.String(maxLength: 50),
                        QuickCode = c.String(maxLength: 200),
                        IdCard = c.String(maxLength: 50),
                        IsMember = c.Boolean(nullable: false),
                        TW = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HS = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        Creater = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MemberRecharges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MemberId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        Name = c.String(maxLength: 50),
                        QuickCode = c.String(maxLength: 200),
                        IdCard = c.String(maxLength: 50),
                        OldPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NewPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PayOrderId = c.Int(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Members", "CustomerId", c => c.Int(nullable: false));
            DropColumn("dbo.Members", "Address");
            DropColumn("dbo.Members", "AddressNow");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Members", "AddressNow", c => c.String(maxLength: 200));
            AddColumn("dbo.Members", "Address", c => c.String(maxLength: 200));
            DropColumn("dbo.Members", "CustomerId");
            DropTable("dbo.MemberRecharges");
            DropTable("dbo.CustomerTemps");
            DropTable("dbo.Customers");
        }
    }
}
