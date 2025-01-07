using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Models.EF
{
    public partial class DefaultConnection : DbContext
    {
        public DefaultConnection()
            : base("name=DefaultConnection")
        {
        }
        public virtual DbSet<Adv> tb_Adv { get; set; }
        //public virtual DbSet<Banners> tb_Banners { get; set; }
        public virtual DbSet<Category> tb_Categories { get; set; }
        public virtual DbSet<Contact> tb_Contact { get; set; }
        public virtual DbSet<News> tb_News { get; set; }
        public virtual DbSet<Order> tb_Order { get; set; }
        public virtual DbSet<OrderDetail> tb_OrderDetail { get; set; }
        public virtual DbSet<Posts> tb_Posts { get; set; }
        public virtual DbSet<Product> tb_Products { get; set; }
        public virtual DbSet<ProductCategory> tb_ProductCategory { get; set; }
        public virtual DbSet<ProductImage> tb_ProductImage { get; set; }
        // public virtual DbSet<Slides> tb_Slides { get; set; }
        // public virtual DbSet<SpecialOffers> tb_SpecialOffers { get; set; }
        public virtual DbSet<Subscribe> tb_Subscribe { get; set; }
        public virtual DbSet<SystemSetting> tb_SystemSetting { get; set; }
        public virtual DbSet<ThongKe> ThongKes { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
