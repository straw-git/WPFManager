namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FinanceBills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddType = c.Int(nullable: false),
                        Things = c.String(maxLength: 500),
                        Remark = c.String(maxLength: 200),
                        BillTime = c.DateTime(nullable: false),
                        StaffId = c.String(maxLength: 50),
                        StaffName = c.String(maxLength: 50),
                        StaffQuickCode = c.String(maxLength: 200),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FinanceTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        TypeId = c.Int(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Goods",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 50),
                        Name = c.String(maxLength: 100),
                        QuickCode = c.String(maxLength: 500),
                        UnitId = c.Int(nullable: false),
                        TypeId = c.Int(nullable: false),
                        Specification = c.String(maxLength: 50),
                        SalePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(maxLength: 200),
                        IsStock = c.Boolean(nullable: false),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelTime = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                "dbo.Payments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CardId = c.String(),
                        Remark = c.String(),
                        Holder = c.String(),
                        PayModelId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelTime = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PaymentLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        PayModelId = c.Int(nullable: false),
                        PaymentId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UseWindowName = c.String(),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PurchasePlans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlanCode = c.String(maxLength: 50),
                        Finished = c.Boolean(nullable: false),
                        Stock = c.Boolean(nullable: false),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelTime = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PurchasePlanItems",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 50),
                        PlanCode = c.String(maxLength: 50),
                        GoodsId = c.String(maxLength: 50),
                        Count = c.Int(nullable: false),
                        Finished = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PurchasePlanLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.String(maxLength: 50),
                        Count = c.Int(nullable: false),
                        PurchasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SupplierId = c.Int(nullable: false),
                        Manufacturer = c.String(maxLength: 200),
                        Remark = c.String(maxLength: 200),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Staffs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 50),
                        Name = c.String(maxLength: 50),
                        QuickCode = c.String(maxLength: 200),
                        IdCard = c.String(maxLength: 50),
                        Sex = c.Int(nullable: false),
                        Address = c.String(maxLength: 500),
                        NowAddress = c.String(maxLength: 500),
                        Phone = c.String(maxLength: 50),
                        QQ = c.String(maxLength: 100),
                        JobPostId = c.Int(nullable: false),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelTime = c.DateTime(nullable: false),
                        Register = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StaffContracts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Write = c.DateTime(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(maxLength: 200),
                        StaffId = c.String(maxLength: 50),
                        Stop = c.Boolean(nullable: false),
                        StopTime = c.DateTime(nullable: false),
                        StopUser = c.Int(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StaffInsurances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Start = c.DateTime(nullable: false),
                        Monthly = c.Boolean(nullable: false),
                        Stop = c.Boolean(nullable: false),
                        End = c.DateTime(nullable: false),
                        Write = c.DateTime(nullable: false),
                        CompanyPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StaffPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompanyName = c.String(maxLength: 200),
                        Remark = c.String(maxLength: 200),
                        StaffId = c.String(maxLength: 50),
                        StopUser = c.Int(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StaffSalaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StaffId = c.String(maxLength: 50),
                        SatffName = c.String(maxLength: 50),
                        StaffQuickCode = c.String(maxLength: 200),
                        Register = c.DateTime(nullable: false),
                        Start = c.DateTime(nullable: false),
                        IsEnd = c.Boolean(nullable: false),
                        End = c.DateTime(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(maxLength: 200),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StaffSalaryOthers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MonthCode = c.String(maxLength: 50),
                        StaffId = c.String(maxLength: 50),
                        Type = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DoTime = c.DateTime(nullable: false),
                        Remark = c.String(maxLength: 200),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StaffSalarySettlements",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 10),
                        StaffCount = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Creator = c.Int(nullable: false),
                        CreatorName = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StaffSalarySettlementLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StaffId = c.String(maxLength: 50),
                        StaffName = c.String(maxLength: 50),
                        StaffQuickCode = c.String(maxLength: 200),
                        MonthCode = c.String(maxLength: 50),
                        BasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsurancePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeductionPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AwardPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GoodsId = c.String(maxLength: 50),
                        StoreId = c.Int(nullable: false),
                        Count = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StockLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        StockType = c.Int(nullable: false),
                        GoodsId = c.String(maxLength: 50),
                        Count = c.Int(nullable: false),
                        Surplus = c.Int(nullable: false),
                        SalePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SupplierId = c.Int(nullable: false),
                        Manufacturer = c.String(maxLength: 200),
                        Remark = c.String(maxLength: 200),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Address = c.String(maxLength: 200),
                        Name = c.String(maxLength: 100),
                        Qualification = c.Boolean(nullable: false),
                        Phone = c.String(),
                        ContactName = c.String(),
                        Creater = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        IsDel = c.Boolean(nullable: false),
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
                        Menus = c.String(),
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
            DropTable("dbo.Users");
            DropTable("dbo.SysDics");
            DropTable("dbo.Suppliers");
            DropTable("dbo.StockLogs");
            DropTable("dbo.Stocks");
            DropTable("dbo.StaffSalarySettlementLogs");
            DropTable("dbo.StaffSalarySettlements");
            DropTable("dbo.StaffSalaryOthers");
            DropTable("dbo.StaffSalaries");
            DropTable("dbo.StaffInsurances");
            DropTable("dbo.StaffContracts");
            DropTable("dbo.Staffs");
            DropTable("dbo.PurchasePlanLogs");
            DropTable("dbo.PurchasePlanItems");
            DropTable("dbo.PurchasePlans");
            DropTable("dbo.PaymentLogs");
            DropTable("dbo.Payments");
            DropTable("dbo.Logs");
            DropTable("dbo.Goods");
            DropTable("dbo.FinanceTypes");
            DropTable("dbo.FinanceBills");
        }
    }
}
