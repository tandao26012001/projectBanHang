using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_ProductColor")]
    public class ProductColor : CommonAbstract
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int ColorId { get; set; } 

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("ColorId")]
        public virtual Color Color { get; set; }
    }
}