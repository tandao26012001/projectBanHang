namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1404 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tb_Product", "ShortDescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_Product", "ShortDescription", c => c.String(maxLength: 250));
        }
    }
}
