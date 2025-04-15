namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCategory1504 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tb_Category", "SeoDescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_Category", "SeoDescription", c => c.String(maxLength: 250));
        }
    }
}
