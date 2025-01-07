using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Models.EF;

namespace projectBanHang.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
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
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}