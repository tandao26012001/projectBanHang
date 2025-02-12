namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase1001 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tb_Product", "IsFeature");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_Product", "IsFeature", c => c.Boolean(nullable: false));
        }
    }
}
