namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProduct1202 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_Product", "SeoKeywords", c => c.String(maxLength: 500));
            DropColumn("dbo.tb_Product", "SeoDescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_Product", "SeoDescription", c => c.String(maxLength: 500));
            AlterColumn("dbo.tb_Product", "SeoKeywords", c => c.String(maxLength: 250));
        }
    }
}
