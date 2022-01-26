namespace DBModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class member_isdelete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "IsDelete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "IsDelete");
        }
    }
}
