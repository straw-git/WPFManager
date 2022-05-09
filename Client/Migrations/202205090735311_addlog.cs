namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addlog : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Logs");
        }
    }
}
