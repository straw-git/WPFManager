namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class plugindlls : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plugins", "DLLs", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plugins", "DLLs");
        }
    }
}
