namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class activity_mj : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MJActivities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        M = c.Decimal(nullable: false, precision: 18, scale: 2),
                        J = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Type = c.Int(nullable: false),
                        MemberTypeId = c.Int(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MJActivities");
        }
    }
}
