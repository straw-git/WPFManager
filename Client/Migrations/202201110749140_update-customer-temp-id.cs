namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecustomertempid : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.CustomerTemps");
            AlterColumn("dbo.CustomerTemps", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.CustomerTemps", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.CustomerTemps");
            AlterColumn("dbo.CustomerTemps", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.CustomerTemps", "Id");
        }
    }
}
