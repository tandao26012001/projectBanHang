namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixCart2010 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tb_Product", "Color");
            DropColumn("dbo.tb_Product", "HexColor");
            DropColumn("dbo.tb_Product", "GroupId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_Product", "GroupId", c => c.Guid());
            AddColumn("dbo.tb_Product", "HexColor", c => c.String(maxLength: 80));
            AddColumn("dbo.tb_Product", "Color", c => c.String(maxLength: 80));
        }
    }
}
