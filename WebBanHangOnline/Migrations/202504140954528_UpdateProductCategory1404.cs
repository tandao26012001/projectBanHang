namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProductCategory1404 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_ProductCategory", "Alias", c => c.String(maxLength: 150));
            AlterColumn("dbo.tb_ProductCategory", "SeoTitle", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.tb_ProductCategory", "SeoDescription", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.tb_ProductCategory", "SeoKeywords", c => c.String(nullable: false, maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_ProductCategory", "SeoKeywords", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_ProductCategory", "SeoDescription", c => c.String(maxLength: 500));
            AlterColumn("dbo.tb_ProductCategory", "SeoTitle", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_ProductCategory", "Alias", c => c.String(nullable: false, maxLength: 150));
        }
    }
}
