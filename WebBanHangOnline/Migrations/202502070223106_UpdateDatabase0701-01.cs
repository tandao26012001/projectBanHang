namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase070101 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tb_ProductImage", "Image");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_ProductImage", "Image", c => c.String());
        }
    }
}
