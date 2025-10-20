namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixColorRelation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_Color",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        HexColor = c.String(maxLength: 10),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        Modifiedby = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.tb_ProductColor", "ColorId", c => c.Int(nullable: false));
            CreateIndex("dbo.tb_ProductColor", "ColorId");
            AddForeignKey("dbo.tb_ProductColor", "ColorId", "dbo.tb_Color", "Id", cascadeDelete: true);
            DropColumn("dbo.tb_ProductColor", "Color");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_ProductColor", "Color", c => c.String(maxLength: 20));
            DropForeignKey("dbo.tb_ProductColor", "ColorId", "dbo.tb_Color");
            DropIndex("dbo.tb_ProductColor", new[] { "ColorId" });
            DropColumn("dbo.tb_ProductColor", "ColorId");
            DropTable("dbo.tb_Color");
        }
    }
}
