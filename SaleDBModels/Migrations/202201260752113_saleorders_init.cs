namespace SaleDBModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class saleorders_init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SaleOrders",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 50),
                        CustomerId = c.Int(nullable: false),
                        CustomerTempId = c.Int(nullable: false),
                        Name = c.String(maxLength: 50),
                        QuickCode = c.String(maxLength: 200),
                        IdCard = c.String(maxLength: 50),
                        Phone = c.String(maxLength: 50),
                        IsMember = c.Boolean(nullable: false),
                        OrderId = c.String(),
                        OrderFinished = c.Boolean(nullable: false),
                        OrderFinishedTime = c.DateTime(nullable: false),
                        OrderFinishUserId = c.Int(nullable: false),
                        OrderFinishUserName = c.String(maxLength: 50),
                        IsPay = c.Boolean(nullable: false),
                        PayPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreateTime = c.DateTime(nullable: false),
                        Creater = c.Int(nullable: false),
                        CreatorName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SaleOrders");
        }
    }
}
