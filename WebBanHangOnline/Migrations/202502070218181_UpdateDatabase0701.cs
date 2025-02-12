namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase0701 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_ProductImage", "ImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_ProductImage", "ImageUrl");
        }
    }
}
