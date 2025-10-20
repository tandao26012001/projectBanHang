namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductColor2010 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_ProductColor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Color = c.String(maxLength: 20),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        Modifiedby = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_Product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_ProductColor", "ProductId", "dbo.tb_Product");
            DropIndex("dbo.tb_ProductColor", new[] { "ProductId" });
            DropTable("dbo.tb_ProductColor");
        }
    }
}
