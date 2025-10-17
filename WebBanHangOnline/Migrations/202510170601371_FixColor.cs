namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixColor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Product", "Color", c => c.String(maxLength: 80));
            DropColumn("dbo.tb_Product", "ColorCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_Product", "ColorCode", c => c.String(maxLength: 80));
            DropColumn("dbo.tb_Product", "Color");
        }
    }
}
