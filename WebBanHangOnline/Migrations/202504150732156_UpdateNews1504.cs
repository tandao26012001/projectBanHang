namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateNews1504 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tb_News", "SeoDescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_News", "SeoDescription", c => c.String());
        }
    }
}
