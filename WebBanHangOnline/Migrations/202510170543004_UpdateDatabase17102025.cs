namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase17102025 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Product", "HexColor", c => c.String(maxLength: 8));
            AddColumn("dbo.tb_Product", "GroupId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_Product", "GroupId");
            DropColumn("dbo.tb_Product", "HexColor");
        }
    }
}
