namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtableattachment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        FromTable = c.Int(nullable: false),
                        DataId = c.Int(nullable: false),
                        SavedPath = c.String(maxLength: 200),
                        SavedName = c.String(maxLength: 100),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelTime = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreatorName = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Attachments");
        }
    }
}
